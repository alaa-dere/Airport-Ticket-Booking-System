using System;
using System.ComponentModel.DataAnnotations;
using TASK2.Models;
using TASK2.Services.Auth;
using TASK2.Services.Manager;
using TASK2.Services.Passengers;
using TASK2.Presentation.Readers;

namespace TASK2.Presentation
{
    public class MainMenu 
    {
        private readonly IAuthService _authService;
        private readonly IManagerService _managerService;
        private readonly IPassengerService _passengerService;
        private readonly IConsoleReader _consoleReader;

        public MainMenu(
            IAuthService authService,
            IManagerService managerService,
            IPassengerService passengerService,
            IConsoleReader consoleReader)
        {
            _authService = authService;
            _managerService = managerService;
            _passengerService = passengerService;
            _consoleReader = consoleReader;
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
                                var managerMenu = new ManagerMenu(_managerService, _consoleReader);
                                managerMenu.Display();
                            }
                        else
                            {
                                var passengerMenu = new PassengerMenu(_passengerService);
                                passengerMenu.Display(new PassengerEmail(user.Email));
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

            try
            {
                _authService.RegisterPassenger(name ?? string.Empty, email ?? string.Empty, password ?? string.Empty);
            }
            catch (ValidationException exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Passenger registered successfully. You can login now.");

            Console.ReadLine();
        }
    }
}
