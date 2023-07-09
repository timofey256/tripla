using System.Net.Http; 
using System.Text.RegularExpressions;
using PuppeteerSharp;
using HtmlAgilityPack;

namespace SkyScannerScraper;

class UrlFormatter {
	private readonly string base_url = "https://www.skyscanner.com/transport"; 
	private readonly string everywhere_endpoint = "flights-from";
	private readonly string city_to_city_endpoint = "flights";

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

class Country {
	public string Name { get; private set; }
	public string Link { get; private set; }
	
	public Country(string name, string link) {
		Name = name;
		Link = link;
	}
}

class PageParser {
	public string Page { get; set; }
		
	private readonly string countryNamePattern = "<span(.)+\">[a-zA-Z]<\\/span>"; 
	private readonly string linkPattern = "/https(.)+";

	public PageParser(string _page) {
		Page = _page;	
	}

	public List<Country> GetAllCities() {
		var content = new HtmlDocument();
		content.LoadHtml(Page);

		var nodes = content.DocumentNode.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("BpkCard_bpk-card"));
		
		List<Country> destinations = extractCardsInfo(nodes,content); 
		return destinations;
	}

	private List<Country> extractCardsInfo(IEnumerable<HtmlNode> nodes, HtmlDocument doc) {
		List<Country> countries = new List<Country>();
		foreach (HtmlNode node in nodes) {
			string card = node.InnerHtml;
	
			// (???) Probably there's a room for speeding up by going through a card only once?
			// but will it be a significant speed increasing tho?
			var countryNode = node.Descendants("span").Where(d => d.GetAttributeValue("class", "").Contains("BpkText_bpk-text__"));
			var linkNode = node.Descendants("a");
			if (!countryNode.Any() || !linkNode.Any()) {
				continue;
			}
			
			var countryName = countryNode.ElementAt(0).InnerHtml;
			var link = linkNode.ElementAt(0).GetAttributeValue("href", "");
			Country country = new Country(countryName, link);	

			countries.Add(country);
		}
		return countries;
	}

	private string extractInfo(string text, string pattern) {
		Regex regexPattern = new Regex(pattern);
		GroupCollection groups = regexPattern.Match(text).Groups;
		string info = groups[1].Value;
		return info;	
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
		string page = Scrapper.GetHTML(skyScannerUrl);
		//Console.WriteLine($"Page: {page} \n-------------");
		//string page = Scrapper.GetPregeneratedHTML(@"./pregeneratedPage.html");
		PageParser parser = new PageParser(page);
		var nodes = parser.GetAllCities();
		foreach (var city in nodes) {
			Console.WriteLine($"Country: {city.Name}, Link: {city.Link}. \n");
		}
	}
}
