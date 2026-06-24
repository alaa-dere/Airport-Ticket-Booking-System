using System;
using TASK2.Services;
using TASK2.Models;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Testing ManagerService: Filter Bookings ===");

        var passengerService = new PassengerService();
        var managerService = new ManagerService();

        passengerService.BookFlight(
            flightId: 1, 
            passengerEmail: "manager.test@najah.edu", 
            passengerName: "Sami", 
            passengerPhone: "123456", 
            selectedClass: FlightClass.FirstClass
        );

        Console.WriteLine("\n[MANAGER] Filtering bookings by Class = FirstClass...");
        var filteredResults = managerService.FilterBookings(flightClass: FlightClass.FirstClass);

        if (filteredResults.Count > 0)
        {
            foreach (var b in filteredResults)
            {
                Console.WriteLine($"-> Found Booking ID: {b.Id} | Email: {b.PassengerEmail} | Class: {b.SelectedClass} | Paid: {b.PricePaid}$");
            }
        }
        else
        {
            Console.WriteLine("No bookings matched the manager's filters.");
        }

        Console.WriteLine("\n=============================================");
    }
}