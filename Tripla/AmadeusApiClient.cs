using System.Net.Http;

namespace Tripla.AmadeusApi;

class AmadeusApiClient {
	private readonly string apiKey; 
       	private readonly string	apiBaseUrl = "https://api.amadeus.com";
	private readonly HttpClient httpClient;
	
	public AmadeusApiClient(string _apiKey) {
		httpClient = new HttpClient();
		apiKey = _apiKey;
	}

	public async Task<string> GetCheapestFlight(string originLocationCode, string destinationLocationCode) {
		string endpoint = "/v1/shopping/flight-offers";
		string queryString = $"originLocationCode={originLocationCode}&destinationLocationCode={destinationLocationCode}";

		httpClient.DefaultRequestHeaders.Clear();
		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
		
		HttpResponseMessage response = await httpClient.GetAsync($"{apiBaseUrl}{endpoint}?{queryString}");

		if (response.IsSuccessStatusCode) {
		    string responseBody = await response.Content.ReadAsStringAsync();
		    return responseBody;
		}
		else {
		    return null;
		}
    }
}
