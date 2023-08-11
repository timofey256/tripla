using Microsoft.AspNetCore.Mvc;
using System;

namespace Tripla.Controllers;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase {
	private readonly AmadeusApiClient client; 
	
	public FlightsController() {
		client = new AmadeusApiClient(Credentials.apiKey, Credentials.apiSecret, 1); // Credentials.cs is an uncommited file where senstitive data is stored
	}

	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] string dateString, [FromQuery] string origin, [FromQuery] string dest) {
        if (!DateTime.TryParse(dateString, out DateTime date))
        {
            return BadRequest("Bad date format");
        }

		var flights = await client.GetFlights(origin, dest, date);
		
		return Ok(flights);
	}
}
