namespace Tripla;

public class Airport {
	public string iataCode { get; set; }
	public string terminal { get; set; }
	public string at { get; set; }
	public const string atFormat = "s"; // "sortable" type of DateTime formatting: yyyy-MM-ddTHH-mm-ss
}
