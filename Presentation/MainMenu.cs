using System;
using TASK2.File_Storage.Parser;
using TASK2.Models;
using TASK2.Services.Auth;

namespace TASK2.Presentation
{
    public class MainMenu 
    {
        private readonly IAuthService _authService;
        private readonly ManagerMenu _managerMenu;
        private readonly PassengerMenu _passengerMenu;
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);

        public MainMenu(
            IAuthService authService,
            ManagerMenu managerMenu,
            PassengerMenu passengerMenu)
        {
            _authService = authService;
            _managerMenu = managerMenu;
            _passengerMenu = passengerMenu;
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
                        var user = Login();

                        if (user == null)
                            break;

                        if (user.Role == UserRole.Manager)
                        {
                            _managerMenu.Display();
                        }
                        else
                        {
                            _passengerMenu.Display(user.Email);
                        }   

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

        private User? Login()
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
                return null;
            }

            var user = _authService.Login(email.Trim(), password);

            if (user == null)
            {
                Console.WriteLine("Invalid email or password.");
                Console.ReadLine();
                return null;
            }

            return user;
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

            if (!Parser.IsValidSimpleValue(name) ||
                !Parser.IsValidSimpleValue(email) ||
                !Parser.IsValidSimpleValue(password))
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
