using Microsoft.AspNetCore.Mvc;
using System;

namespace Tripla;

[ApiController]
[Route("[controller]")]
public class FlightsController : ControllerBase {
	[HttpGet]
	//public IActionResult GetFlight([FromQuery] string dateStr, [FromQuery] string origin, [FromQuery] string dest) {
	public IActionResult GetFlight() {
		DateTime date;
		if (!DateTime.TryParseExact(dateStr, 
					    "yyyy-MM-dd", 
					    null, 
					    System.Globalization.DateTimeStyles.None, 
					    out DateTime parsedDate)) 
		{
			return BadRequest("Invalid date was provided.");
		}

		var flightsData = new
		{
            Date = "date",
            Origin = "origin",
            Destination = "dest",
        };

        return Ok(flightsData);
	}
}
