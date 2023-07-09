using System.Net.Http; 
using System.Text.RegularExpressions;
using PuppeteerSharp;
using HtmlAgilityPack;

namespace SkyScannerScraper;

class UrlFormatter {
	private readonly string base_url = "https://www.skyscanner.cz/doprava"; 
	private readonly string everywhere_endpoint = "lety-z";
	private readonly string city_to_city_endpoint = "lety";

	private readonly Dictionary<string, string> cityToCode = new Dictionary<string, string>() {
		{"Prague", "prg"},
		{"Milan", "mila"}
	};

	private readonly Dictionary<string, string> codeToCity = new Dictionary<string, string>() {
		{"prg", "Prague"},
		{"mila", "Milan"}
	};

	private string startCityCode;
	public string startCity {
		private get { return codeToCity[startCityCode]; } 
		set {
			if (!cityToCode.ContainsKey(value)) {
				throw new ArgumentException($"You can't find a trip through {value}.");
			}
			startCityCode = cityToCode[value];
		}
	}
	
	private string destCityCode;
	public string destCity {
		private get { return codeToCity[destCityCode]; } 
		set {
			if (!cityToCode.ContainsKey(value)) {
				throw new ArgumentException($"You can't find a trip through {value}.");
			}
			destCityCode = cityToCode[value];
		}
	}
	
	public string startDate { get; set; }

	public string endDate { get; set; }	
	
	public string getUrl(bool is_everywhere = false) {
		if (is_everywhere)
			return $"{base_url}/{everywhere_endpoint}/{startCityCode}/{startDate}/{endDate}";
		return "";
	}
}

static class Scrapper {
	private static string[] userAgents = new[] {
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36",
		"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0",
		"Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36"
	};
	
	public static string GetHTML(string url) {
		var response = CallUrl(url);
		string page = response.Result;
		page = removeScripts(page);
		return page;
	}
	
	/// <summary>
	/// Returns page that was retrieved previously in order to not scrap the website once again.
	/// Only for testing!
	/// </summary>
	public static string GetPregeneratedHTML(string filePath) {
		string text = File.ReadAllText(filePath);
		text = removeScripts(text);
		return text;	
	}

	private static string removeScripts(string page) {
		string pattern = "<script(.)+/script>";
		page = Regex.Replace(page, pattern, "");
		return page;
	}

	private static async Task<string> CallUrl(string url) {
		await SetupBrowser();

		using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
		{
		    Headless = true,
		    Args = new[] { "--disable-infobars" }		
		});

		var page = await browser.NewPageAsync();
		await ConfigurePage(page);
		await ConfigureRequestInterceptions(page);
		await page.GoToAsync(url, new NavigationOptions
		{
		    WaitUntil = new[] { WaitUntilNavigation.Load, 
				    	WaitUntilNavigation.Networkidle0, 
				    	WaitUntilNavigation.DOMContentLoaded }
		});

		var content = await page.GetContentAsync();
        	return content;
    	}

    	private static async Task SetupBrowser() {
        	using var browserFetcher = new BrowserFetcher();
        	await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    	}

	private static async Task ConfigurePage(IPage page) {
        	await page.SetViewportAsync(new ViewPortOptions { Width = 1366, Height = 768 });
		var randomUserAgent = userAgents[new Random().Next(0, userAgents.Length)];
		await page.SetUserAgentAsync(randomUserAgent);
        	await page.SetOfflineModeAsync(false);
    	}

	private static async Task ConfigureRequestInterceptions(IPage page) {
		await page.SetRequestInterceptionAsync(true);
		page.Request += async (sender, e) =>
		{
		    if (e.Request.ResourceType == ResourceType.Image || e.Request.ResourceType == ResourceType.Font)
		    {
			await e.Request.AbortAsync();
		    }
		    else
		    {
			await e.Request.ContinueAsync();
		    }
		};
	} 
}

class PageParser {
	public string Page { get; set; }
	
	public PageParser(string _page) {
		Page = _page;	
	}

	public IEnumerable<HtmlNode> GetAllItems() {
		var content = new HtmlDocument();
		content.LoadHtml(Page);

		var cards = content.DocumentNode.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("BpkCard_bpk-card"));
		return cards;
	}
}

class Program {
	public static void Main(string[] args) {
		var formatter = new UrlFormatter();
		formatter.startDate = "230724";
		formatter.endDate = "230730";
		formatter.startCity = "Prague";
		string skyScannerUrl = formatter.getUrl(true);
		Console.WriteLine($"Generated URL: {skyScannerUrl}");
		//string page = Scrapper.GetHTML(skyScannerUrl);
		string page = Scrapper.GetPregeneratedHTML(@"./pregeneratedPage.html");
		PageParser parser = new PageParser(page);
		var nodes = parser.GetAllItems();
		foreach (var node in nodes) {
			Console.WriteLine(node.InnerHtml);
		}
	}
}
