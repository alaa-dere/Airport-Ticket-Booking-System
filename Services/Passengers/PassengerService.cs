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

        public bool Book(BookingRequest bookingRequest)
        {
            if (string.IsNullOrWhiteSpace(bookingRequest.PassengerEmail) ||
                string.IsNullOrWhiteSpace(bookingRequest.PassengerName) ||
                string.IsNullOrWhiteSpace(bookingRequest.PassengerPhone) ||
                !Parser.IsValidSimpleValue(bookingRequest.PassengerEmail) ||
                !Parser.IsValidSimpleValue(bookingRequest.PassengerName) ||
                !Parser.IsValidSimpleValue(bookingRequest.PassengerPhone))
            {
                return false;
            }

            var flight = _flightService.GetAll().FirstOrDefault(f => f.Id == bookingRequest.FlightId);
            
            if (flight == null)
            {
                return false;
            }

            var alreadyBooked = _bookingService.GetAll().Any(b =>
                b.FlightId == bookingRequest.FlightId &&
                b.Passenger.Email!.Equals(bookingRequest.PassengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
            {
                return false;
            }

            decimal finalPrice = flight.GetPriceForClass(bookingRequest.SelectedClass);

            var newBooking = new Booking
            {
                FlightId = bookingRequest.FlightId,
                Passenger = new Passenger
                {
                    Email = bookingRequest.PassengerEmail,
                    Name = bookingRequest.PassengerName,
                    Phone = bookingRequest.PassengerPhone
                },
                SelectedClass = bookingRequest.SelectedClass,
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
