using System.Net.Http; 
using PuppeteerSharp;

namespace SkyScannerScraper;

class UrlFormatter {
	private readonly string base_url = "https://www.skyscanner.cz/doprava/"; 
	private readonly string everywhere_endpoint = "lety-z/";
	private readonly string city_to_city_endpoint = "lety/";

	private string start_city;
	private string dest_city;
	private string start_date;
	private string end_date;

	public UrlFormatter() {
		start_city = String.Empty;
		dest_city = String.Empty;
		start_date = String.Empty;	
		end_date = String.Empty;	
	}

	public void setDate(string start, string finish) {
		start_date = start + "/";
		end_date = finish + "/";
	}

	public void setStartCity(string city) {
		start_city = city + "/";
	}
	
	public void setDestCity(string city) {
		dest_city = city + "/";
	}
	
	public string getUrl(bool is_everywhere = false) {
		if (is_everywhere)
			return base_url + everywhere_endpoint + start_city + start_date + end_date;
		return "";
	}
}
static class Scrapper
{
	public static string GetHTML(string url)
	{
		var response = CallUrl(url);
		string page = response.Result;
		return page;
	}

	private static async Task<string> CallUrl(string url)
	{
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

    	private static async Task SetupBrowser()
    	{
        	using var browserFetcher = new BrowserFetcher();
        	await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    	}

	private static async Task ConfigurePage(IPage page)
   	{
        	await page.SetViewportAsync(new ViewPortOptions { Width = 1366, Height = 768 });
		var userAgents = new[]
		{
		    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36",
		    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0",
		    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36"
		};
		var randomUserAgent = userAgents[new Random().Next(0, userAgents.Length)];
		await page.SetUserAgentAsync(randomUserAgent);
        	await page.SetOfflineModeAsync(false); // Set to true for offline mode
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

class Program {
	public static void Main(string[] args) {
		var formatter = new UrlFormatter();
		formatter.setDate("230724", "230730");
		formatter.setStartCity("prg");
		string skyScannerUrl = formatter.getUrl(true);
		string page = Scrapper.GetHTML(skyScannerUrl);
		Console.WriteLine(page);
	}
}
