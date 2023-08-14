using PuppeteerSharp;
using System.Text.RegularExpressions;

namespace SkyScannerScraper;

static class Scrapper {
	static readonly int indexUserAgent = 0;
	private static string[] userAgents = new[]
	{
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36",
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0",
	    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36",
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 OPR/80.0.4170.16",
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 Edg/94.0.992.38",
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 EdgA/94.0.992.38",
	    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 EdgA/94.0.992.38",
	    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 OPR/80.0.4170.16",
	    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 Edg/94.0.992.38",
	    "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36 EdgA/94.0.992.38",
	    "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.61 Safari/537.36",
	    "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:93.0) Gecko/20100101 Firefox/93.0",
	    "Mozilla/5.0 (X11; Fedora; Linux x86_64; rv:93.0) Gecko/20100101 Firefox/93.0"
	};
	
	public static string GetHTML(string url) {
		var response = CallUrl(url);
		string page = response.Result;
		if (page.Contains("Are you a person or a robot?")) {
			Console.WriteLine("ERROR: Script was detected.");
		}
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
		await SetCookies(page);	

		await page.GoToAsync(url, new NavigationOptions
		{
		    WaitUntil = new[] { WaitUntilNavigation.Load, 
				    	WaitUntilNavigation.Networkidle0, 
				    	WaitUntilNavigation.DOMContentLoaded }
		});

		var content = await page.GetContentAsync();
        	return content;
    	}

	private static async Task SetCookies(IPage page) {
		var cookies = new Dictionary<string, string> {
		    {"avoid_banana_results", "true"},
		    {"_pxvid", "0c6e9527-2105-11ee-ade7-0b4fd9244e4a"},
		    {"cookieName2", "0c6e9527-2105-11ee-ade7-0b4fd9244e4a"},
		    {"__Secure-anon_token", "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImM3ZGZlYjI2LTlmZjUtNDY4OC1iYjc3LWRiNTY2NWUyNjFkZSJ9.eyJhenAiOiIyNWM3MGZmZDAwN2JkOGQzODM3NyIsImh0dHBzOi8vc2t5c2Nhbm5lci5uZXQvbG9naW5UeXBlIjoiYW5vbnltb3VzIiwiaHR0cHM6Ly9za3lzY2FubmVyLm5ldC91dGlkIjoiZDIwYzY3NjEtZDQyZS00YTgyLWIzMjEtMzRhODEyYTA2MDYwIiwiaHR0cHM6Ly9za3lzY2FubmVyLm5ldC9jc3JmIjoiOTI0YTU2MWU3Y2FmN2U3NTc5YWEzYmE4NmZhZTgxYmQiLCJodHRwczovL3NreXNjYW5uZXIubmV0L2p0aSI6IjQ3ZTA5ZjdmLTQzZWEtNDJiMy05NGJmLWZkYzZhMDNkZWRlZiIsImlhdCI6MTY4OTIwMTYwNiwiZXhwIjoxNzUyMjczNjA2LCJhdWQiOiJodHRwczovL2dhdGV3YXkuc2t5c2Nhbm5lci5uZXQvaWRlbnRpdHkiLCJpc3MiOiJodHRwczovL3d3dy5za3lzY2FubmVyLm5ldC9zdHRjL2lkZW50aXR5L2p3a3MvcHJvZC8ifQ.ac8hcwts46FoVwPe8VCcAUpmN7JB3i62I_smXvz092cy1Ot6tEZGce1Pt_0jd9UIURsysrpuYoGwPh-OHfoDfd6BDW3AGg-6OeSEPz5cBySSH3YN_yovIV9aQhY_IevN3AacICQZzBfcGDxAfRj_VwIurZd7-2Jy8BcbMYH7Svpt0HsaceL5vln8G8Hoc-xNllKDCwCgjlkbJDeO62LfR9pyVSDlUqLw1AighO01MgE9bzf9pWfaxNt9d3VAOU7Wly617nZZdwo5Dk2jR28ncENMZz7tI5qITxVacSAKVzqszoA5YByjiwfUAOCj86rE-h26aUKWZvSW5XvAPiLrQg"},
		    //{"__Secure-ska", "a7dc82df-6e04-4123-9157-fff683e05a50"},
		    {"device_guid", "a7dc82df-6e04-4123-9157-fff683e05a50"},
		    {"preferences", "d20c6761d42e4a82b32134a812a06060"}
		};

		foreach (var cookie in cookies) {
			Console.WriteLine("going to add cookie");
		    	
			bool temp = false;
			if (cookie.Key == "__Secure-anon_token") {
				temp = true;
			}
			await page.SetCookieAsync(new CookieParam {
			Name = cookie.Key,
			Value = cookie.Value,
			Domain = "www.skyscanner.com", // Set the appropriate domain for the cookie
			Path = "/", // Set the appropriate path for the cookie
			Secure = temp, // Set to true if the cookie requires a secure connection
			HttpOnly = temp // Set to true if the cookie is accessible only via HTTP
		    });
		}
	}

    	private static async Task SetupBrowser() {
        	using var browserFetcher = new BrowserFetcher();
        	await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
    	}

	private static async Task ConfigurePage(IPage page) {
        	await page.SetViewportAsync(new ViewPortOptions { Width = 1920, Height = 1080 });
		var randomUserAgent = userAgents[indexUserAgent];
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
