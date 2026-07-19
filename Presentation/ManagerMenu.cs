using System;
using TASK2.Services.Manager;
using TASK2.Models;
using TASK2.Presentation.Readers;
using TASK2.Presentation.Renderers;
namespace TASK2.Presentation
{
    public class ManagerMenu
    {
        private readonly IMenuInputReader _inputReader;
        private readonly IManagerService _managerService;
        private readonly IMenuRenderer _renderer;

        public ManagerMenu(IManagerService managerService, IConsoleReader consoleReader)
        {
            _managerService = managerService;
            _inputReader = new MenuInputReader(consoleReader);
            _renderer = new MenuRenderer();
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

            BookingFilter filter = _inputReader.ReadBookingFilter();
            var bookings = _managerService.FilterBookings(filter);

            _renderer.DisplayFilteredBookings(bookings);
            _renderer.WaitForReturn();
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
                _renderer.DisplayValidationErrors(result.Errors);
            }

            _renderer.WaitForReturn();
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
                _renderer.DisplayValidationErrors(errors);
            }

            _renderer.WaitForReturn();
        }

        private void HandleValidationGuide()
        {
            Console.Clear();
            Console.WriteLine("=== Dynamic Flight Validation Constraints Guide ===\n");

            var guide = _managerService.GetFlightValidationDetails();

            _renderer.DisplayValidationGuide(guide);
            _renderer.WaitForReturn();
        }

        private void HandleViewAllFlights()
        {
            Console.Clear();
            Console.WriteLine("=== All Flights ===");

            var flights = _managerService.GetAll();

            _renderer.DisplayManagerFlights(flights);
            _renderer.WaitForReturn();
        }
    }
}