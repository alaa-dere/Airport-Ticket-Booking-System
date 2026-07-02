using System;
using TASK2.Presentation;
using TASK2.Services.Auth;
using TASK2.Services.Manager;
using TASK2.Services.Passengers;
using TASK2.Models;
using TASK2.File_Storage.Bookings;
using TASK2.File_Storage.Flights;
using TASK2.File_Storage.Users;
using TASK2.Services.Bookings;
using TASK2.Services.Flights;
using TASK2.Services.Validation;
using TASK2.Presentation.Readers;
class Program
{
    static void Main(string[] args)
    {
        IConsoleReader consoleReader = new ConsoleReader();
        IUserRepository userRepository = new UserRepository();
        FlightRepository flightBookingRepository = new FlightRepository();
        IFlightRepository flightRepository = flightBookingRepository;
        IBookingRepository bookingRepository = flightBookingRepository;
        IAuthService authService = new AuthService(userRepository);
        IValidationService validationService = new ValidationService(flightRepository);
        IFlightService flightService = new FlightService(flightRepository);
        IBookingService bookingService = new BookingService(bookingRepository);
        IManagerService managerService = new ManagerService(flightService, bookingService, validationService);
        IPassengerService passengerService = new PassengerService(flightService, bookingService);

        ManagerMenu managerMenu = new ManagerMenu(managerService, consoleReader);
        PassengerMenu passengerMenu = new PassengerMenu(passengerService);
        MainMenu mainMenu = new MainMenu(authService, managerService, passengerService, consoleReader);
        mainMenu.Display();
    }
}
