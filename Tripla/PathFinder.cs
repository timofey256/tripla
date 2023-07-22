namespace Tripla;

class PathFinder {	
	private readonly AmadeusApiClient client;	
	
	public PathFinder(string apiKey, string apiSecret) {
		client = new AmadeusApiClient(apiKey, apiSecret, 1);
	}

	public async Task<FlightOfferResponse> BuildItinerary(List<Airport> points) {
		List<FlightOffer> flights = new List<FlightOffer>();

		for (int i=0; i < points.Count-1; i++) {
			var cur = points[i];
			var next = points[i+1];

			string origin = cur.iataCode;
		       	string destination = next.iataCode;
			DateTime date = DateTime.Parse(cur.at);

			FlightOfferResponse response = await client.GetFlights(origin, destination, date);	
			FlightOffer offer = response.data[0];
			flights.Add(offer);	
		}
		
		var itinerary = new FlightOfferResponse() {
			data = flights
		};
		
		return itinerary;
	}
}
