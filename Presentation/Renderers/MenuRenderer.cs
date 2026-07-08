using TASK2.Models;

namespace TASK2.Presentation.Renderers;

public class MenuRenderer : IMenuRenderer
{
    public void DisplayFilteredBookings(IReadOnlyCollection<Booking> bookings)
    {
        Console.WriteLine("\n--- Filtered Bookings Results ---");

        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings matched the filters.");
            return;
        }

        foreach (var b in bookings)
        {
            Console.WriteLine(
                $"[Booking ID: {b.Id}] Passenger: {b.Passenger.Email} | " +
                $"Flight ID: {b.FlightId} | Class: {b.SelectedClass} | " +
                $"Price Paid: {b.PricePaid}$"
            );
        }
    }

    public void DisplayPassengerBookings(IReadOnlyCollection<Booking> bookings)
    {
        if (bookings.Count == 0)
        {
            Console.WriteLine("You have no active bookings.");
            return;
        }

        foreach (var b in bookings)
        {
            Console.WriteLine(
                $"Booking ID: {b.Id} | Flight ID: {b.FlightId} | " +
                $"Class: {b.SelectedClass} | Paid: {b.PricePaid}$"
            );
        }
    }

    public void DisplayPassengerFlights(IReadOnlyCollection<Flight> flights)
    {
        if (flights.Count == 0)
        {
            Console.WriteLine("No flights matched your search criteria.");
            return;
        }

        DisplayFlights(flights);
    }

    public void DisplayManagerFlights(IReadOnlyCollection<Flight> flights)
    {
        if (flights.Count == 0)
        {
            Console.WriteLine("No flights found.");
            return;
        }

        DisplayFlights(flights);
    }

    public void DisplayValidationErrors(IReadOnlyCollection<FileValidationError> errors)
    {
        Console.WriteLine("----------------------------------------");

        foreach (var err in errors)
        {
            Console.WriteLine(
                $"[Row {err.RowNumber}] Field: {err.FieldName} -> {err.ErrorMessage}"
            );
        }

        Console.WriteLine("----------------------------------------");
    }

    public void DisplayValidationGuide(IReadOnlyCollection<FieldValidationInfo> guide)
    {
        Console.WriteLine("--------------------------------------------------------------------------------");
        Console.WriteLine(string.Format("| {0,-18} | {1,-12} | {2,-42} |",
            "Field Name", "Data Type", "Validation Constraints"));
        Console.WriteLine("--------------------------------------------------------------------------------");

        foreach (var info in guide)
        {
            Console.WriteLine(string.Format("| {0,-18} | {1,-12} | {2,-42} |",
                info.FieldName, info.Type, info.Constraints));
        }

        Console.WriteLine("--------------------------------------------------------------------------------");
    }

    public void WaitForReturn()
    {
        Console.WriteLine("\nPress Enter to return.");
        Console.ReadLine();
    }

    private void DisplayFlights(IReadOnlyCollection<Flight> flights)
    {
        foreach (var f in flights)
        {
            Console.WriteLine(
                $"[ID: {f.Id}] {f.DepartureCountry} ({f.DepartureAirport}) -> " +
                $"{f.DestinationCountry} ({f.ArrivalAirport}) | " +
                $"Date: {f.DepartureTime:yyyy-MM-dd} | Base Price: {f.BasePrice}$"
            );
        }
    }
}