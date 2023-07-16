namespace Tripla;

class UrlFormatter {
	private readonly string baseUrl; 

	public UrlFormatter(string _baseUrl) {
		baseUrl = _baseUrl;
	}
	
	public string GetUrl(Dictionary<string, string> parameters) {
		string url = baseUrl;
		
		if (parameters.Count != 0) {
			url += "?";
		}
		
		foreach (var p in parameters) {
			url += $"{p.Key}={p.Value}&";
		}

		return url;
	}
}
