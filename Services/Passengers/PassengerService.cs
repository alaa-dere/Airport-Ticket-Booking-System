using System;
using System.Collections.Generic;
using System.Linq; 
using TASK2.File_Storage.Parser;
using TASK2.Services.Flights;
using TASK2.Services.Bookings;
using TASK2.Models;

namespace TASK2.Services.Passengers
{
    public class PassengerService : IPassengerService
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);

        public PassengerService(IFlightService flightService,
            IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter)
        {
            return _flightService.SearchFlights(filter);
        }

        public bool Book(int flightId, string passengerEmail, string passengerName, string passengerPhone, FlightClass selectedClass)
        {
            if (string.IsNullOrWhiteSpace(passengerEmail) ||
                string.IsNullOrWhiteSpace(passengerName) ||
                string.IsNullOrWhiteSpace(passengerPhone) ||
                !Parser.IsValidSimpleValue(passengerEmail) ||
                !Parser.IsValidSimpleValue(passengerName) ||
                !Parser.IsValidSimpleValue(passengerPhone))
            {
                return false;
            }

            var flight = _flightService.GetAll().FirstOrDefault(f => f.Id == flightId);
            
            if (flight == null)
            {
                return false;
            }

            var alreadyBooked = _bookingService.GetAll().Any(b =>
                b.FlightId == flightId &&
                b.Passenger.Email!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
            {
                return false;
            }

            decimal finalPrice = flight.GetPriceForClass(selectedClass);

            var newBooking = new Booking
            {
                FlightId = flightId,
                Passenger = new Passenger
                {
                    Email = passengerEmail,
                    Name = passengerName,
                    Phone = passengerPhone
                },
                SelectedClass = selectedClass,
                PricePaid = finalPrice
            };

            _bookingService.Add(newBooking);
            return true;
        }
        public bool Cancel(int bookingId, string passengerEmail)
        {
        var booking = _bookingService.GetAll().FirstOrDefault(b => b.Id == bookingId && b.Passenger.Email!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));
            
            if (booking == null)
                return false;

            _bookingService.Delete(bookingId);
            return true;
        }
       
        public bool Modify(int bookingId, string passengerEmail, int newFlightId, FlightClass newClass)
        {
        var booking = _bookingService.GetAll().FirstOrDefault(b => b.Id == bookingId && b.Passenger.Email!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));
            
            if (booking == null)
                return false;

            var flight = _flightService.GetAll().FirstOrDefault(f => f.Id == newFlightId);
            
            if (flight == null)
                return false;

            var alreadyBooked = _bookingService.GetAll().Any(b =>
                b.Id != bookingId &&
                b.FlightId == newFlightId &&
                b.Passenger.Email!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
                return false;

            booking.FlightId = newFlightId;
            booking.SelectedClass = newClass;
            booking.PricePaid = flight.GetPriceForClass(newClass);
            
            _bookingService.Update(booking);
            return true;
        }

        public IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail)
        {
            return _bookingService.GetAll()
             .Where(b => b.Passenger.Email!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase))
             .ToList();
        }
    }
}