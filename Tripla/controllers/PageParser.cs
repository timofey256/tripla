using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SkyScannerScraper;

static class PageParser {
	private static readonly string countryNamePattern = "<span(.)+\">[a-zA-Z]<\\/span>"; 
	private static readonly string linkPattern = "/https(.)+";

	public static List<Country> GetAllCards(string Page) {
		var content = new HtmlDocument();
		content.LoadHtml(Page);

		var nodes = content.DocumentNode.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("BpkCard_bpk-card"));
		
		List<Country> destinations = extractCardsInfo(nodes,content); 
		return destinations;
	}

	private static List<Country> extractCardsInfo(IEnumerable<HtmlNode> nodes, HtmlDocument doc) {
		List<Country> countries = new List<Country>();
		foreach (HtmlNode node in nodes) {
			string card = node.InnerHtml;
	
			// (???) Probably there's a room for speeding up by going through a card only once?
			// but will it be a significant speed increasing tho?
			var countryNode = node.Descendants("span").Where(d => d.GetAttributeValue("class", "").Contains("BpkText_bpk-text__"));
			var linkNode = node.Descendants("a");
			var priceNode = node.Descendants("div").Where(d => d.GetAttributeValue("class", "").Contains("PriceDescription_container"));
			if (!countryNode.Any() || !linkNode.Any()) {
				continue;
			}
			
			string countryName = countryNode.ElementAt(0).InnerHtml;
			string priceStr = priceNode.ElementAt(0).Descendants("span").ElementAt(1).InnerHtml;
			int price = int.Parse(Regex.Replace(priceStr, "[^0-9.]", ""));
			string link = linkNode.ElementAt(0).GetAttributeValue("href", "");
			Country country = new Country(countryName, price, link);	

			countries.Add(country);
		}
		return countries;
	}

	private static string extractInfo(string text, string pattern) {
		Regex regexPattern = new Regex(pattern);
		GroupCollection groups = regexPattern.Match(text).Groups;
		string info = groups[1].Value;
		return info;	
	}
}
