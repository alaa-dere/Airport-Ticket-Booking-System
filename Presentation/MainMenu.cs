using System;
using TASK2.File_Storage;
using TASK2.Models;
using TASK2.Services;

namespace TASK2.Presentation
{
    public class MainMenu
    {
        private readonly AuthService _authService;

        public MainMenu()
        {
            _authService = new AuthService();
        }

        public void Display()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   Welcome to Airport Booking System");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register as Passenger");
                Console.WriteLine("3. Exit Application");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HandleLogin();
                        break;

                    case "2":
                        HandlePassengerRegistration();
                        break;

                    case "3":
                        exit = true;
                        Console.WriteLine("Thank you for using our system. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void HandleLogin()
        {
            Console.Clear();
            Console.WriteLine("=== Login ===");

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Email and password are required.");
                Console.ReadLine();
                return;
            }

            var user = _authService.Login(email.Trim(), password);

            if (user == null)
            {
                Console.WriteLine("Invalid email or password.");
                Console.ReadLine();
                return;
            }

            if (user.Role == UserRole.Manager)
            {
                var managerMenu = new ManagerMenu();
                managerMenu.Display();
            }
            else
            {
                var passengerMenu = new PassengerMenu();
                passengerMenu.Display(user.Email);
            }
        }

        private void HandlePassengerRegistration()
        {
            Console.Clear();
            Console.WriteLine("=== Passenger Registration ===");

            Console.Write("Name: ");
            string? name = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Name, email, and password are required.");
                Console.ReadLine();
                return;
            }

            if (!CsvUtility.IsValidSimpleValue(name) ||
                !CsvUtility.IsValidSimpleValue(email) ||
                !CsvUtility.IsValidSimpleValue(password))
            {
                Console.WriteLine("Name, email, and password cannot contain commas.");
                Console.ReadLine();
                return;
            }

            var isRegistered = _authService.RegisterPassenger(name.Trim(), email.Trim(), password);

            Console.WriteLine(isRegistered
                ? "Passenger registered successfully. You can login now."
                : "Registration failed. Email is already registered.");

            Console.ReadLine();
        }
    }
}
