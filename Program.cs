using System;
using TASK2.Services;
using TASK2.Models;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Testing PassengerService: Search and Book ===");

        var passengerService = new PassengerService();

        Console.WriteLine("\n[STEP 1] Searching for flights to 'Egypt'...");
        var searchResults = passengerService.SearchFlights(null, "Egypt", null, 200);

        if (searchResults.Count > 0)
        {
            var targetFlight = searchResults[0];
            Console.WriteLine($"Found Flight ID: {targetFlight.Id} | Base Price: {targetFlight.BasePrice}$");

            Console.WriteLine("\n[STEP 2] Attempting to book this flight as Business Class...");
            
            bool isBooked = passengerService.BookFlight(
                flightId: targetFlight.Id,
                passengerEmail: "alaa@najah.edu",
                passengerName: "Alaa",
                passengerPhone: "+9705990000",
                selectedClass: FlightClass.Business
            );

            if (isBooked)
            {
                Console.WriteLine("Booking Successful! Check your bookings.csv file.");
            }
            else
            {
                Console.WriteLine("Booking Failed! Flight not found.");
            }
        }
        else
        {
            Console.WriteLine("No flights found to test booking. Make sure flights.csv has data.");
        }

        Console.WriteLine("\n=============================================");
    }
}