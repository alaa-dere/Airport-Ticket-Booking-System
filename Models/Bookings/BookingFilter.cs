
namespace TASK2.Models;
public class BookingFilter
{
    public int? FlightId { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? DepartureCountry { get; set; }
    public string? DestinationCountry { get; set; }
    public DateTime? DepartureDate { get; set; }
    public string? DepartureAirport { get; set; }
    public string? ArrivalAirport { get; set; }
    public string? PassengerEmail { get; set; }
    public FlightClass? FlightClass { get; set; }
}