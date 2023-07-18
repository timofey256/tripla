namespace Tripla;

public class Segment {
	public Airport departure { get; set; }
	public Airport arrival { get; set; }
	public string carrierCode { get; set; }
	public string number { get; set; }
	public string duration { get; set; }

	public override string ToString() {
		return $"Departure: {departure}. Arrival: {arrival}. Duration: {duration}";
	}
}
