using System.Net.Http; 
using System.Text.RegularExpressions;

namespace SkyScannerScraper;

class Program {
	public static void Main(string[] args) {
		var formatter = new UrlFormatter();
		formatter.startDate = "230724";
		formatter.endDate = "230730";
		formatter.startCity = "Prague";
		string skyScannerUrl = formatter.getUrl(true);
		Console.WriteLine($"Generated URL: {skyScannerUrl}");
		//string page = Scrapper.GetHTML(skyScannerUrl);
		//Console.WriteLine($"Page: {page} \n-------------");
		string page = Scrapper.GetPregeneratedHTML(@"./pregeneratedPage.html");
		PageParser parser = new PageParser(page);
		var nodes = parser.GetAllCities();
		foreach (var city in nodes) {
			Console.WriteLine($"Country: {city.Name}, \nPrice: {city.Price} \n Link: {city.Link}. \n");
		}
	}
}
