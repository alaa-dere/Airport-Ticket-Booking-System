using System;
using TASK2.File_Storage.Parser;
using TASK2.Services.Passengers;
using TASK2.Models;

namespace TASK2.Presentation
{
    public class PassengerMenu
    {
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);
        private readonly IPassengerService _passengerService;

        public PassengerMenu(IPassengerService passengerService)
        {
            _passengerService = passengerService;
        } 


        public void Display(string email)
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine($"=== Passenger Menu (Logged in as: {email}) ===");
                Console.WriteLine("1. Search for Available Flights");
                Console.WriteLine("2. Book a Flight");
                Console.WriteLine("3. View My Bookings");
                Console.WriteLine("4. Modify a Booking");
                Console.WriteLine("5. Cancel a Booking");
                Console.WriteLine("6. Logout");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleSearchFlights();
                        break;

                    case "2":
                        HandleBookFlight(email);
                        break;

                    case "3":
                        HandleViewMyBookings(email);
                        break;

                    case "4":
                        HandleModifyBooking(email);
                        break;

                    case "5":
                        HandleCancelBooking(email);
                        break;

                    case "6":
                        logout = true;
                        Console.WriteLine("Logging out...");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void HandleSearchFlights()
        {
            Console.Clear();
            Console.WriteLine("=== Search Available Flights ===");

            var flights = GetFlightSearchResults();

            Console.WriteLine("\n--- Available Flights Found ---");
            DisplayFlights(flights);

            Console.WriteLine("\nPress Enter to return to menu.");
            Console.ReadLine();
        }

        private IReadOnlyCollection<Flight> GetFlightSearchResults()
        {
            Console.Write("Enter Departure Country (or press Enter to skip): ");
            string? departureCountry = ReadOptionalText();

            Console.Write("Enter Destination Country (or press Enter to skip): ");
            string? destinationCountry = ReadOptionalText();

            Console.Write("Enter Departure Airport (or press Enter to skip): ");
            string? departureAirport = ReadOptionalText();

            Console.Write("Enter Arrival Airport (or press Enter to skip): ");
            string? arrivalAirport = ReadOptionalText();

            Console.Write("Enter Departure Date yyyy-MM-dd (or press Enter to skip): ");
            DateTime? departureDate = ReadOptionalDate();

            Console.Write("Enter Max Price (or press Enter to skip): ");
            decimal? maxPrice = ReadOptionalDecimal();

            Console.WriteLine("Select Class:");
            Console.WriteLine("1. Economy");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. First Class");
            Console.WriteLine("Press Enter to skip");
            Console.Write("Your choice: ");
            FlightClass? selectedClass = ReadOptionalFlightClass();

            return _passengerService.SearchFlights(
                new FlightFilter
                {
                    DepartureCountry = departureCountry,
                    DestinationCountry = destinationCountry,
                    DepartureAirport = departureAirport,
                    ArrivalAirport = arrivalAirport,
                    DepartureDate = departureDate,
                    MaxPrice = maxPrice,
                    FlightClass = selectedClass
                }
            );
        }

        private void HandleBookFlight(string email)
        {
            Console.Clear();
            Console.WriteLine("=== Book a Flight ===");

            var flights = GetFlightSearchResults();

            Console.WriteLine("\n--- Search Results ---");
            DisplayFlights(flights);

            if (flights.Count == 0)
            {
                Console.WriteLine("\nNo flights available to book.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nEnter Flight ID to book: ");
            if (!int.TryParse(Console.ReadLine(), out int flightId))
            {
                Console.WriteLine("Invalid Flight ID.");
                Console.ReadLine();
                return;
            }

            if (!flights.Any(f => f.Id == flightId))
            {
                Console.WriteLine("Selected Flight ID is not in the search results.");
                Console.ReadLine();
                return;
            }

            string name = ReadRequiredSimpleText("Enter Passenger Name: ");
            string phone = ReadRequiredSimpleText("Enter Passenger Phone: ");

            Console.WriteLine("\nSelect Class:");
            Console.WriteLine("1. Economy");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. First Class");
            Console.Write("Your choice: ");

            FlightClass? selectedClass = ReadOptionalFlightClass();

            if (selectedClass == null)
            {
                Console.WriteLine("Invalid class selected.");
                Console.ReadLine();
                return;
            }

            bool success = _passengerService.Book(
                flightId,
                email,
                name,
                phone,
                selectedClass.Value
            );

            Console.WriteLine(success
                ? "\nBooking completed successfully!"
                : "\nBooking failed. Flight not found or already booked.");

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleViewMyBookings(string email)
        {
            Console.Clear();
            Console.WriteLine("=== My Bookings ===");

            var bookings = _passengerService.GetMyBookings(email);
            DisplayBookings(bookings);

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleModifyBooking(string email)
        {
            Console.Clear();
            Console.WriteLine("=== Modify Booking ===");

            var myBookings = _passengerService.GetMyBookings(email);
            DisplayBookings(myBookings);

            if (myBookings.Count == 0)
            {
                Console.WriteLine("\nYou have no bookings to modify.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nEnter Booking ID to modify: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nNow search for the new flight:");
            var flights = GetFlightSearchResults();

            Console.WriteLine("\n--- Available Flights ---");
            DisplayFlights(flights);

            if (flights.Count == 0)
            {
                Console.WriteLine("\nNo flights available.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nEnter New Flight ID: ");
            if (!int.TryParse(Console.ReadLine(), out int newFlightId))
            {
                Console.WriteLine("Invalid Flight ID.");
                Console.ReadLine();
                return;
            }

            if (!flights.Any(f => f.Id == newFlightId))
            {
                Console.WriteLine("Selected Flight ID is not in the search results.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nSelect New Class:");
            Console.WriteLine("1. Economy");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. First Class");
            Console.Write("Your choice: ");

            FlightClass? newClass = ReadOptionalFlightClass();

            if (newClass == null)
            {
                Console.WriteLine("Invalid class selected.");
                Console.ReadLine();
                return;
            }

            bool success = _passengerService.Modify(
                bookingId,
                email,
                newFlightId,
                newClass.Value
            );

            Console.WriteLine(success
                ? "\nBooking modified successfully!"
                : "\nModification failed. Check Booking ID, Flight ID, or duplicate booking.");

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleCancelBooking(string email)
        {
            Console.Clear();
            Console.WriteLine("=== Cancel Booking ===");

            var myBookings = _passengerService.GetMyBookings(email);
            DisplayBookings(myBookings);

            if (myBookings.Count == 0)
            {
                Console.WriteLine("\nYou have no bookings to cancel.");
                Console.ReadLine();
                return;
            }

            Console.Write("\nEnter Booking ID to cancel: ");
            if (!int.TryParse(Console.ReadLine(), out int bookingId))
            {
                Console.WriteLine("Invalid Booking ID.");
                Console.ReadLine();
                return;
            }

            bool success = _passengerService.Cancel(bookingId, email);

            Console.WriteLine(success
                ? "\nBooking canceled successfully!"
                : "\nCancellation failed. Booking not found.");

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void DisplayFlights(IReadOnlyCollection<Flight> flights)
        {
            if (flights.Count == 0)
            {
                Console.WriteLine("No flights matched your search criteria.");
                return;
            }

            foreach (var f in flights)
            {
                Console.WriteLine(
                    $"[ID: {f.Id}] {f.DepartureCountry} ({f.DepartureAirport}) -> " +
                    $"{f.DestinationCountry} ({f.ArrivalAirport}) | " +
                    $"Date: {f.DepartureTime:yyyy-MM-dd} | Base Price: {f.BasePrice}$"
                );
            }
        }

        private void DisplayBookings(IReadOnlyCollection<Booking> bookings)
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

        private string? ReadOptionalText()
        {
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        private string ReadRequiredSimpleText(string message)
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

                if (!Parser.IsValidSimpleValue(input))
                {
                    Console.WriteLine("This field cannot contain commas.");
                    continue;
                }

                return input;
            }
        }

        private decimal? ReadOptionalDecimal()
        {
            string? input = Console.ReadLine();
            return decimal.TryParse(input, out decimal value) ? value : null;
        }

        private DateTime? ReadOptionalDate()
        {
            string? input = Console.ReadLine();
            return DateTime.TryParse(input, out DateTime date) ? date.Date : null;
        }

        private FlightClass? ReadOptionalFlightClass()
        {
            string? input = Console.ReadLine();

            return input switch
            {
                "1" => FlightClass.Economy,
                "2" => FlightClass.Business,
                "3" => FlightClass.FirstClass,
                _ => null
            };
        }
    }
}
