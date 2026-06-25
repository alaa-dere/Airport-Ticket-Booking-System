using System;

namespace TASK2.Presentation
{
    public class MainMenu
    {
        private const string ManagerPassword = "admin123";

        public void Display()
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("   Welcome to Airport Booking System");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Login as Passenger");
                Console.WriteLine("2. Login as Manager");
                Console.WriteLine("3. Exit Application");
                Console.Write("\nSelect your role: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("\nEnter your Email to continue: ");
                        string? email = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(email))
                        {
                            Console.WriteLine("Email cannot be empty.");
                            Console.ReadLine();
                            break;
                        }

                        var passengerMenu = new PassengerMenu();
                        passengerMenu.Display(email.Trim());
                        break;

                    case "2":
                        Console.Write("\nEnter Manager Password: ");
                        string? password = Console.ReadLine();

                        if (password == ManagerPassword)
                        {
                            var managerMenu = new ManagerMenu();
                            managerMenu.Display();
                        }
                        else
                        {
                            Console.WriteLine("Wrong password!");
                            Console.ReadLine();
                        }
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
    }
}