using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace SkyScannerScraper;

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
