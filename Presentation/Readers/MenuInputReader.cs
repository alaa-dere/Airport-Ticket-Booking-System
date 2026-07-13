using TASK2.Extensions;
using TASK2.Models;

namespace TASK2.Presentation.Readers;

public class MenuInputReader : IMenuInputReader
{
    private readonly IConsoleReader _consoleReader;

    public MenuInputReader(IConsoleReader consoleReader)
    {
        _consoleReader = consoleReader;
    }

    /// <inheritdoc />
    public BookingFilter ReadBookingFilter()
    {
        Console.Write("Enter Passenger Email (or press Enter to skip): ");
        string? passengerEmail = _consoleReader.ReadOptionalText();

        Console.Write("Enter Flight ID (or press Enter to skip): ");
        int? flightId = _consoleReader.ReadOptionalInt();

        Console.Write("Enter Max Price (or press Enter to skip): ");
        decimal? maxPrice = _consoleReader.ReadOptionalDecimal();

        Console.Write("Enter Departure Country (or press Enter to skip): ");
        string? departureCountry = _consoleReader.ReadOptionalText();

        Console.Write("Enter Destination Country (or press Enter to skip): ");
        string? destinationCountry = _consoleReader.ReadOptionalText();

        Console.Write("Enter Departure Airport (or press Enter to skip): ");
        string? departureAirport = _consoleReader.ReadOptionalText();

        Console.Write("Enter Arrival Airport (or press Enter to skip): ");
        string? arrivalAirport = _consoleReader.ReadOptionalText();

        Console.Write("Enter Departure Date yyyy-MM-dd (or press Enter to skip): ");
        DateTime? departureDate = _consoleReader.ReadOptionalDate();

        FlightClass? selectedClass = ReadOptionalFlightClassFilter();

        return new BookingFilter
        {
            PassengerEmail = passengerEmail,
            FlightId = flightId,
            MaxPrice = maxPrice,
            DepartureCountry = departureCountry,
            DestinationCountry = destinationCountry,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            DepartureDate = departureDate,
            FlightClass = selectedClass
        };
    }

    /// <inheritdoc />
    public FlightFilter ReadFlightFilter()
    {
        Console.Write("Enter Departure Country (or press Enter to skip): ");
        string? departureCountry = _consoleReader.ReadOptionalText();

        Console.Write("Enter Destination Country (or press Enter to skip): ");
        string? destinationCountry = _consoleReader.ReadOptionalText();

        Console.Write("Enter Departure Airport (or press Enter to skip): ");
        string? departureAirport = _consoleReader.ReadOptionalText();

        Console.Write("Enter Arrival Airport (or press Enter to skip): ");
        string? arrivalAirport = _consoleReader.ReadOptionalText();

        Console.Write("Enter Departure Date yyyy-MM-dd (or press Enter to skip): ");
        DateTime? departureDate = _consoleReader.ReadOptionalDate();

        Console.Write("Enter Max Price (or press Enter to skip): ");
        decimal? maxPrice = _consoleReader.ReadOptionalDecimal();

        FlightClass? selectedClass = ReadOptionalFlightClassFilter();

        return new FlightFilter
        {
            DepartureCountry = departureCountry,
            DestinationCountry = destinationCountry,
            DepartureAirport = departureAirport,
            ArrivalAirport = arrivalAirport,
            DepartureDate = departureDate,
            MaxPrice = maxPrice,
            FlightClass = selectedClass
        };
    }

    /// <inheritdoc />
    public string ReadRequiredSimpleText(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("This field is required.");
                continue;
            }

            input = input.Trim();

            if (!input.IsValidSimpleValue())
            {
                Console.WriteLine("This field cannot contain commas.");
                continue;
            }

            return input;
        }
    }

    /// <inheritdoc />
    public bool TryReadFlightIdFromSearchResults(
        IReadOnlyCollection<Flight> flights,
        string prompt,
        out int flightId)
    {
        Console.Write(prompt);
        if (!int.TryParse(Console.ReadLine(), out flightId))
        {
            Console.WriteLine("Invalid Flight ID.");
            Console.ReadLine();
            return false;
        }

        int selectedFlightId = flightId;

        if (!flights.Any(f => f.Id == selectedFlightId))
        {
            Console.WriteLine("Selected Flight ID is not in the search results.");
            Console.ReadLine();
            return false;
        }

        return true;
    }

    /// <inheritdoc />
    public bool TryReadBookingIdToModify(out int bookingId)
    {
        Console.Write("\nEnter Booking ID to modify: ");
        if (int.TryParse(Console.ReadLine(), out bookingId))
            return true;

        Console.WriteLine("Invalid Booking ID.");
        Console.ReadLine();
        return false;
    }

    /// <inheritdoc />
    public bool TryReadRequiredFlightClass(string heading, out FlightClass flightClass)
    {
        Console.WriteLine(heading);
        Console.WriteLine("1. Economy");
        Console.WriteLine("2. Business");
        Console.WriteLine("3. First Class");
        Console.Write("Your choice: ");

        FlightClass? selectedClass = _consoleReader.ReadOptionalFlightClass();

        if (selectedClass == null)
        {
            Console.WriteLine("Invalid class selected.");
            Console.ReadLine();
            flightClass = default;
            return false;
        }

        flightClass = selectedClass.Value;
        return true;
    }

    private FlightClass? ReadOptionalFlightClassFilter()
    {
        Console.WriteLine("Select Class:");
        Console.WriteLine("1. Economy");
        Console.WriteLine("2. Business");
        Console.WriteLine("3. First Class");
        Console.WriteLine("Press Enter to skip");
        Console.Write("Your choice: ");
        return _consoleReader.ReadOptionalFlightClass();
    }
}