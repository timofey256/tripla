using Microsoft.AspNetCore.Mvc;
using System;

namespace Tripla.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase {
	private readonly AmadeusApiClient client; 
	
	public FlightsController() {
		// Credentials.cs is an uncommited file where senstitive data is stored
		client = new AmadeusApiClient(Credentials.apiKey, Credentials.apiSecret, 1); 
	}

	[HttpGet("single")]
	public async Task<IActionResult> Get([FromQuery] string dateString, [FromQuery] string origin, [FromQuery] string dest) {
		if (!DateTime.TryParse(dateString, out DateTime date))
		{
		    return BadRequest("Bad date format");
		}

		var flights = await client.GetFlights(origin, dest, date);
			
		return Ok(flights);
	}

	[HttpGet("route")]
	public async Task<IActionResult> Get([FromQuery] List<FlightInfo> flights) {
		var itinerary = new List<FlightOfferResponse>();

		foreach (var point in flights) {
			Console.WriteLine($"date: {point.DateString}. origin: {point.Origin}. dest: {point.Dest}");

			if (!DateTime.TryParse(point.DateString, out DateTime date))
			{
			    return BadRequest("Bad date format");
			}

			var flightTicket = await client.GetFlights(point.Origin, point.Dest, date);
			itinerary.Add(flightTicket);
		}

		return Ok(itinerary);
	}
}
