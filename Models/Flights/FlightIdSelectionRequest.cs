namespace TASK2.Models;

public class FlightIdSelectionRequest
{
    public required IReadOnlyCollection<Flight> Flights { get; set; }
    public required string Prompt { get; set; }
}