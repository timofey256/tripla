using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace Tripla;

class AmadeusApiClient {
	private readonly string apiKey; 
	private readonly string apiSecret; 
       	private const string apiBaseUrl = "https://test.api.amadeus.com/v2/shopping/flight-offers";
	private const string accessTokenUrl = "https://test.api.amadeus.com/v1/security/oauth2/token";
	private readonly HttpClient httpClient;
	
	private string accessToken;
       	private DateTime tokenExpireTime { get; set; }	
	
	private const int maxOffers = 1;

	public AmadeusApiClient(string _apiKey, string _apiSecret) {
		httpClient = new HttpClient();
		apiKey = _apiKey;
		apiSecret = _apiSecret;
		tokenExpireTime = DateTime.Now;
		accessToken = null;
	}

	public async Task<string> GetFlights(string originCode, string destinationCode, string departureDate, int adults = 1) {
		await checkIfTokenIsValid();

		Console.WriteLine($"Access token: {accessToken}");
		
		var parameters = new Dictionary<string, string>() {
			{ "originLocationCode", originCode },
			{ "destinationLocationCode", destinationCode },
			{ "departureDate", departureDate },
			{ "adults", adults.ToString() },
			{ "max", maxOffers.ToString() }
		};
		UrlFormatter urlFormatter = new UrlFormatter(apiBaseUrl);
		string url = urlFormatter.GetUrl(parameters);		
		
		string response = await sendFlightsRequest(url);
		return response;
	}
	
	private async Task<string> sendFlightsRequest(string url) {
		setHeaders();

		string response = null;
		try {	
			response = await httpClient.GetStringAsync(url);
		}
		catch (Exception error) {
			Console.WriteLine(error.Message);
		}
		
		Console.WriteLine(response);
		return response;
	}

	private async Task checkIfTokenIsValid() {
		if (DateTime.Compare(tokenExpireTime, DateTime.Now) > 0)  {
			return;
		}

		TokenResponse tokenResponse = await GetAccessToken();	
		accessToken = tokenResponse.access_token;
		tokenExpireTime = DateTime.Now.AddSeconds(tokenResponse.expires_in);			
	}
	
	private void setHeaders() {	
		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
		httpClient.DefaultRequestHeaders.Add("Accept", $"application/json");
	}

	private async Task<TokenResponse> GetAccessToken() {
		TokenResponse tokenResponse = null;
		try {
			var content = getRequestContentForAccessToken();	
			var response = await httpClient.PostAsync(accessTokenUrl, content);
			if (response.IsSuccessStatusCode) {
   				string data = await response.Content.ReadAsStringAsync();
				tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(data);
		    	}
			else {
				Console.WriteLine($"Request failed with status code {response.StatusCode}.");
		    	}
		}
		catch (Exception ex) {
		    Console.WriteLine(ex.Message);
		}	
		return tokenResponse;
	}

	private FormUrlEncodedContent getRequestContentForAccessToken() {
		FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
		{
		    { "grant_type", "client_credentials" },
		    { "client_id", apiKey },
		    { "client_secret", apiSecret }
		});
		return content;
	}

	private class TokenResponse {
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
