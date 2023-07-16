using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace Tripla {

class AmadeusApiClient {
	private readonly string apiKey; 
	private readonly string apiSecret; 
	private string token; 
       	private readonly string	apiBaseUrl = "https://test.api.amadeus.com/v2/shopping/flight-offers";
	private readonly HttpClient httpClient;
	
	private const int maxOffers = 1;

	public AmadeusApiClient(string _apiKey, string _apiSecret) {
		httpClient = new HttpClient();
		apiKey = _apiKey;
		apiSecret = _apiSecret;
		token = null;
	}

	public async Task<string> GetFlights(string originCode, string destinationCode, string departureDate, int adults = 1) {
		var localToken = await GetAccessToken();
		
		var parameters = new Dictionary<string, string>() {
			{ "originLocationCode", originCode },
			{ "destinationLocationCode", destinationCode },
			{ "departureDate", departureDate },
			{ "adults", adults.ToString() },
			{ "max", maxOffers.ToString() }
		};
		
		UrlFormatter urlFormatter = new UrlFormatter(apiBaseUrl);
		string url = urlFormatter.GetUrl(parameters);	
		httpClient.DefaultRequestHeaders.Clear();
        	
		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {localToken}");
		httpClient.DefaultRequestHeaders.Add("Accept", $"application/json");
		
		Console.WriteLine($"Url: {url}");	
		string response = null;
		try {	
			response = await httpClient.GetStringAsync(url);
		}
		catch (Exception error) {
			Console.WriteLine(error.Message);
		}
		return response;
	}
	
	private async Task<string> GetAccessToken() {
		var url = "https://test.api.amadeus.com/v1/security/oauth2/token";

		// Set the request content
		var content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
		    { "grant_type", "client_credentials" },
		    { "client_id", apiKey },
		    { "client_secret", apiSecret }
		});
		
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
		
		string accessToken = "";
		try {
		    	// Send the POST request
			var response = await httpClient.PostAsync(url, content);
			Console.WriteLine(response);
		    	// Check the response status code
			if (response.IsSuccessStatusCode) {
				// Parse the response JSON
   				string data = await response.Content.ReadAsStringAsync();
				var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(data);
				accessToken = tokenResponse.access_token;
				int expiresIn = tokenResponse.expires_in;

				Console.WriteLine($"Access Token: {accessToken}");
				Console.WriteLine($"Expires In: {expiresIn} seconds");
		    	}
			else {
				Console.WriteLine($"Request failed with status code {response.StatusCode}.");
		    	}
		}
		catch (Exception ex) {
		    Console.WriteLine("Request error: " + ex.Message);
		}	
		Console.WriteLine(accessToken);
		return accessToken;
	}
	
	public class TokenResponse {
    		public string access_token { get; set; }
    		public int expires_in { get; set; }
	}	
	
	private class FlightOfferResponse {
		public List<FlightOffer> data {get; set;}
	}

	private class FlightOffer {
		public Price price { get; set; }
		public List<string> validatingAirlineCodes { get; set; }
	}

	private class Price {
		public string total { get; set; }
		public string currency { get; set; }
	}
}

}
