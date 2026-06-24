using TASK2.Services;
using TASK2.Models;

var service = new PassengerService();

// 1. Search Flights
Console.WriteLine("=== Search Flights ===");
var flights = service.SearchFlights(departureCountry: "Jordan");
foreach (var f in flights)
    Console.WriteLine($"{f.Id} | {f.DepartureCountry} → {f.DestinationCountry} | {f.DepartureTime:yyyy-MM-dd} | {f.BasePrice}$");

// 2. Book Flight
Console.WriteLine("\n=== Book Flight ===");
var result = service.BookFlight(1, "sara@gmail.com", "Sara Ahmed", "0599123456", FlightClass.Business);
Console.WriteLine(result ? "Booking Success!" : "Booking Failed!");

// 3. Get My Bookings
Console.WriteLine("\n=== My Bookings ===");
var bookings = service.GetMyBookings("sara@gmail.com");
foreach (var b in bookings)
    Console.WriteLine($"{b.Id} | Flight {b.FlightId} | {b.SelectedClass} | {b.PricePaid}$");