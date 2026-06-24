using System;
using TASK2.Services;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Testing PassengerService: SearchFlights ===");

        var passengerService = new PassengerService();

        // Test Scenario: Search for flights to Egypt with a maximum base price of 200
        Console.WriteLine("\n[TEST 1] Searching for flights to 'Egypt' with Max Price = 200:");
        var searchResults = passengerService.SearchFlights(null, "Egypt", null, 200);

        if (searchResults.Count == 0)
        {
            Console.WriteLine("No flights found matching the criteria.");
        }
        else
        {
            foreach (var flight in searchResults)
            {
                Console.WriteLine($"-> Flight ID: {flight.Id} | From: {flight.DepartureCountry} To: {flight.DestinationCountry} | Base Price: {flight.BasePrice}$");
            }
        }

        Console.WriteLine("\n=============================================");
    }
}