using System;
using TASK2.Services.Passengers;
using TASK2.Models;
using TASK2.Presentation.Readers;
using TASK2.Presentation.Renderers;

namespace TASK2.Presentation
{
    public class PassengerMenu
    {
        private readonly IMenuInputReader _inputReader;
        private readonly IPassengerService _passengerService;
        private readonly IMenuRenderer _renderer;

        public PassengerMenu(IPassengerService passengerService)
        {
            _passengerService = passengerService;
            _inputReader = new MenuInputReader(new ConsoleReader());
            _renderer = new MenuRenderer();
        } 


        public void Display(PassengerEmail email)
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
            _renderer.DisplayPassengerFlights(flights);

            _renderer.WaitForReturn();
            Console.ReadLine();
        }

        private IReadOnlyCollection<Flight> GetFlightSearchResults()
        {
            FlightFilter filter = _inputReader.ReadFlightFilter();
            return _passengerService.SearchFlights(filter);
        }

        private void HandleBookFlight(PassengerEmail email)
        {
            Console.Clear();
            Console.WriteLine("=== Book a Flight ===");

            var flights = SearchAndDisplayFlights("\n--- Search Results ---");

            if (!EnsureFlightsAvailableToBook(flights))
                return;

            if (!_inputReader.TryReadFlightIdFromSearchResults(flights, "\nEnter Flight ID to book: ", out int flightId))
                return;

            string name = _inputReader.ReadRequiredSimpleText("Enter Passenger Name: ");
            string phone = _inputReader.ReadRequiredSimpleText("Enter Passenger Phone: ");

            if (!_inputReader.TryReadRequiredFlightClass("\nSelect Class:", out FlightClass selectedClass))
                return;

            bool success = BookFlightForPassenger(flightId, email, name, phone, selectedClass);

            DisplayBookingResult(success);
            _renderer.WaitForReturn();
        }

        private void HandleViewMyBookings(PassengerEmail email)
        {
            Console.Clear();
            Console.WriteLine("=== My Bookings ===");

            var bookings = _passengerService.GetMyBookings(email.Value);
            _renderer.DisplayPassengerBookings(bookings);

            _renderer.WaitForReturn();
        }

        private void HandleModifyBooking(PassengerEmail email)
        {
            Console.Clear();
            Console.WriteLine("=== Modify Booking ===");

            var myBookings = DisplayBookingsAvailableForModification(email);

            if (!EnsureBookingsAvailableToModify(myBookings))
                return;

            if (!_inputReader.TryReadBookingIdToModify(out int bookingId))
                return;

            Console.WriteLine("\nNow search for the new flight:");
            var flights = SearchAndDisplayFlights("\n--- Available Flights ---");

            if (!EnsureFlightsAvailableToModify(flights))
                return;

            if (!_inputReader.TryReadFlightIdFromSearchResults(flights, "\nEnter New Flight ID: ", out int newFlightId))
                return;

            if (!_inputReader.TryReadRequiredFlightClass("\nSelect New Class:", out FlightClass newClass))
                return;

            bool success = ModifyPassengerBooking(bookingId, email, newFlightId, newClass);

            DisplayModificationResult(success);
            _renderer.WaitForReturn();
        }

        private IReadOnlyCollection<Flight> SearchAndDisplayFlights(string heading)
        {
            var flights = GetFlightSearchResults();

            Console.WriteLine(heading);
            _renderer.DisplayPassengerFlights(flights);

            return flights;
        }

        private bool EnsureFlightsAvailableToBook(IReadOnlyCollection<Flight> flights)
        {
            if (flights.Count != 0)
                return true;

            Console.WriteLine("\nNo flights available to book.");
            Console.ReadLine();
            return false;
        }

        private bool EnsureFlightsAvailableToModify(IReadOnlyCollection<Flight> flights)
        {
            if (flights.Count != 0)
                return true;

            Console.WriteLine("\nNo flights available.");
            Console.ReadLine();
            return false;
        }

        private bool BookFlightForPassenger(
            int flightId,
            PassengerEmail email,
            string name,
            string phone,
            FlightClass selectedClass)
        {
            return _passengerService.Book(new BookingRequest
            {
                FlightId = flightId,
                PassengerEmail = email.Value,
                PassengerName = name,
                PassengerPhone = phone,
                SelectedClass = selectedClass
            });
        }

        private void DisplayBookingResult(bool success)
        {
            Console.WriteLine(success
                ? "\nBooking completed successfully!"
                : "\nBooking failed. Flight not found or already booked.");
        }

        private IReadOnlyCollection<Booking> DisplayBookingsAvailableForModification(PassengerEmail email)
        {
            var myBookings = _passengerService.GetMyBookings(email.Value);
            _renderer.DisplayPassengerBookings(myBookings);

            return myBookings;
        }

        private bool EnsureBookingsAvailableToModify(IReadOnlyCollection<Booking> myBookings)
        {
            if (myBookings.Count != 0)
                return true;

            Console.WriteLine("\nYou have no bookings to modify.");
            Console.ReadLine();
            return false;
        }

        private bool ModifyPassengerBooking(
            int bookingId,
            PassengerEmail email,
            int newFlightId,
            FlightClass newClass)
        {
            return _passengerService.Modify(
                bookingId,
                email.Value,
                newFlightId,
                newClass
            );
        }

        private void DisplayModificationResult(bool success)
        {
            Console.WriteLine(success
                ? "\nBooking modified successfully!"
                : "\nModification failed. Check Booking ID, Flight ID, or duplicate booking.");
        }

        private void HandleCancelBooking(PassengerEmail email)
        {
            Console.Clear();
            Console.WriteLine("=== Cancel Booking ===");

            var myBookings = _passengerService.GetMyBookings(email.Value);
            _renderer.DisplayPassengerBookings(myBookings);

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

            bool success = _passengerService.Cancel(bookingId, email.Value);

            Console.WriteLine(success
                ? "\nBooking canceled successfully!"
                : "\nCancellation failed. Booking not found.");

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }
    }
}
