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
