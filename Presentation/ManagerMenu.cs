using System;
using TASK2.Services;
using TASK2.Models;

namespace TASK2.Presentation
{
    public class ManagerMenu
    {
        private readonly ManagerService _managerService;

        public ManagerMenu()
        {
            _managerService = new ManagerService();
        }

        public void Display()
        {
            bool logout = false;

            while (!logout)
            {
                Console.Clear();
                Console.WriteLine("=== Manager Menu ===");
                Console.WriteLine("1. Filter Bookings");
                Console.WriteLine("2. Batch Upload Flights From CSV");
                Console.WriteLine("3. Validate Imported Flight Data");
                Console.WriteLine("4. View Flight Model Validation Guide");
                Console.WriteLine("5. View All Flights");
                Console.WriteLine("6. Logout");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleFilterBookings();
                        break;

                    case "2":
                        HandleBatchUpload();
                        break;

                    case "3":
                        HandleValidateImportedData();
                        break;

                    case "4":
                        HandleValidationGuide();
                        break;

                    case "5":
                        HandleViewAllFlights();
                        break;

                    case "6":
                        logout = true;
                        Console.WriteLine("Logging out from Manager Panel...");
                        break;

                    default:
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void HandleFilterBookings()
        {
            Console.Clear();
            Console.WriteLine("=== Filter Bookings ===");

            Console.Write("Enter Passenger Email (or press Enter to skip): ");
            string? passengerEmail = ReadOptionalText();

            Console.Write("Enter Flight ID (or press Enter to skip): ");
            int? flightId = ReadOptionalInt();

            Console.Write("Enter Max Price (or press Enter to skip): ");
            decimal? maxPrice = ReadOptionalDecimal();

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

            Console.WriteLine("Select Class:");
            Console.WriteLine("1. Economy");
            Console.WriteLine("2. Business");
            Console.WriteLine("3. First Class");
            Console.WriteLine("Press Enter to skip");
            Console.Write("Your choice: ");
            FlightClass? selectedClass = ReadOptionalFlightClass();

            var bookings = _managerService.FilterBookings(
                passengerEmail: passengerEmail,
                flightId: flightId,
                maxPrice: maxPrice,
                departureCountry: departureCountry,
                destinationCountry: destinationCountry,
                departureAirport: departureAirport,
                arrivalAirport: arrivalAirport,
                departureDate: departureDate,
                flightClass: selectedClass
            );

            Console.WriteLine("\n--- Filtered Bookings Results ---");

            if (bookings.Count == 0)
            {
                Console.WriteLine("No bookings matched the filters.");
            }
            else
            {
                foreach (var b in bookings)
                {
                    Console.WriteLine(
                        $"[Booking ID: {b.Id}] Passenger: {b.PassengerEmail} | " +
                        $"Flight ID: {b.FlightId} | Class: {b.SelectedClass} | " +
                        $"Price Paid: {b.PricePaid}$"
                    );
                }
            }

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleBatchUpload()
        {
            Console.Clear();
            Console.WriteLine("=== Batch Upload Flights From CSV ===");

            Console.Write("Enter CSV file path: ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Path cannot be empty.");
                Console.ReadLine();
                return;
            }

            var result = _managerService.BatchUploadFlights(filePath.Trim());

if (result.IsSuccess)
            {
                Console.WriteLine("\nAll flights imported successfully.");
            }
            else
            {
                Console.WriteLine("\nImport failed. Validation errors:");
                Console.WriteLine("----------------------------------------");

                foreach (var err in result.Errors)
                {
                    Console.WriteLine(
                        $"[Row {err.RowNumber}] Field: {err.FieldName} -> {err.ErrorMessage}"
                    );
                }

                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleValidateImportedData()
        {
            Console.Clear();
            Console.WriteLine("=== Validate Imported Flight Data ===");

            Console.Write("Enter CSV file path to validate: ");
            string? filePath = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(filePath))
            {
                Console.WriteLine("Path cannot be empty.");
                Console.ReadLine();
                return;
            }

            var errors = _managerService.ValidateImportedFlightData(filePath.Trim());

            if (errors.Count == 0)
            {
                Console.WriteLine("\nFile data is valid. No errors found.");
            }
            else
            {
                Console.WriteLine("\nValidation Errors:");
                Console.WriteLine("----------------------------------------");

                foreach (var err in errors)
                {
                    Console.WriteLine(
                        $"[Row {err.RowNumber}] Field: {err.FieldName} -> {err.ErrorMessage}"
                    );
                }

                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleValidationGuide()
        {
            Console.Clear();
            Console.WriteLine("=== Dynamic Flight Validation Constraints Guide ===\n");

            var guide = _managerService.GetFlightValidationDetails();

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
            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private void HandleViewAllFlights()
        {
            Console.Clear();
            Console.WriteLine("=== All Flights ===");

            var flights = _managerService.GetAllFlights();

            if (flights.Count == 0)
            {
                Console.WriteLine("No flights found.");
            }
            else
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

            Console.WriteLine("\nPress Enter to return.");
            Console.ReadLine();
        }

        private string? ReadOptionalText()
        {
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

        private int? ReadOptionalInt()
        {
            string? input = Console.ReadLine();
            return int.TryParse(input, out int value) ? value : null;
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