using System.ComponentModel.DataAnnotations;
using TASK2.Validation;

namespace TASK2.Models;

public class Flight
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Flight ID must be a valid positive integer.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Departure Country is required.")]
    public string? DepartureCountry { get; set; }

    [Required(ErrorMessage = "Destination Country is required.")]
    public string? DestinationCountry { get; set; }

    [Required(ErrorMessage = "Departure Airport is required.")]
    public string? DepartureAirport { get; set; }

    [Required(ErrorMessage = "Arrival Airport is required.")]
    public string? ArrivalAirport { get; set; }

    [Required]
    [FutureOrToday(ErrorMessage = "Departure Time cannot be in the past. Must be today or future.")]
    public DateTime DepartureTime { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Base Price must be a valid positive number.")]
    public decimal BasePrice { get; set; } 

    public decimal GetPriceForClass(FlightClass flightClass)
    {
        return flightClass switch
        {
            FlightClass.Economy => BasePrice,
            FlightClass.Business => BasePrice * 1.5m,  
            FlightClass.FirstClass => BasePrice * 2.5m, 
            _ => BasePrice
        };
    }
}
