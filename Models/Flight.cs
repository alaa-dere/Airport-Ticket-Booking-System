namespace TASK2.Models;
public class Flight
{
    public int Id { get; set; }
    public string? DepartureCountry { get; set; }
    public string? DestinationCountry { get; set; }
    public string? DepartureAirport { get; set; }
    public string? ArrivalAirport { get; set; }
    public DateTime DepartureTime { get; set; }
    public decimal Price { get; set; }

}