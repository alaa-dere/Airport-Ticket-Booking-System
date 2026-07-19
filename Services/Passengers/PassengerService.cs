using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq; 
using TASK2.Extensions;
using TASK2.Services.Flights;
using TASK2.Services.Bookings;
using TASK2.Models;

namespace TASK2.Services.Passengers
{
    public class PassengerService : IPassengerService
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public PassengerService(IFlightService flightService,
            IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter)
        {
            return _flightService.SearchFlights(filter);
        }

        /// <inheritdoc />
        public Booking Book(BookingRequest bookingRequest)
        {
            if (string.IsNullOrWhiteSpace(bookingRequest.PassengerEmail) ||
                string.IsNullOrWhiteSpace(bookingRequest.PassengerName) ||
                string.IsNullOrWhiteSpace(bookingRequest.PassengerPhone) ||
                !bookingRequest.PassengerEmail.IsValidSimpleValue() ||
                !bookingRequest.PassengerName.IsValidSimpleValue() ||
                !bookingRequest.PassengerPhone.IsValidSimpleValue())
            {
                throw new ValidationException("Passenger email, name, and phone are required and must not contain commas or new lines.");
            }

            var flight = _flightService.GetAll().FirstOrDefault(f => f.Id == bookingRequest.FlightId);
            
            if (flight == null)
            {
                throw new KeyNotFoundException("Flight not found.");
            }

            var alreadyBooked = _bookingService.GetAll().Any(b =>
                b.FlightId == bookingRequest.FlightId &&
                b.Passenger.Email.Value.Equals(bookingRequest.PassengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
            {
                throw new InvalidOperationException("Passenger already booked this flight.");
            }

            decimal finalPrice = flight.GetPriceForClass(bookingRequest.SelectedClass);

            var newBooking = new Booking
            {
                FlightId = bookingRequest.FlightId,
                Passenger = new Passenger(
                    new Email(bookingRequest.PassengerEmail),
                    bookingRequest.PassengerName,
                    bookingRequest.PassengerPhone),
                SelectedClass = bookingRequest.SelectedClass,
                PricePaid = finalPrice
            };

            return _bookingService.Add(newBooking);
        }

        /// <inheritdoc />
        public void Cancel(int bookingId, string passengerEmail)
        {
            _bookingService.Delete(bookingId);
        }
       
        /// <inheritdoc />
        public bool Modify(ModifyBookingRequest modifyBookingRequest)
        {
        var booking = _bookingService.GetAll().FirstOrDefault(b => b.Id == modifyBookingRequest.BookingId && b.Passenger.Email.Value.Equals(modifyBookingRequest.PassengerEmail, StringComparison.OrdinalIgnoreCase));
            
            if (booking == null)
                return false;

            var flight = _flightService.GetAll().FirstOrDefault(f => f.Id == modifyBookingRequest.NewFlightId);
            
            if (flight == null)
                return false;

            var alreadyBooked = _bookingService.GetAll().Any(b =>
                b.Id != modifyBookingRequest.BookingId &&
                b.FlightId == modifyBookingRequest.NewFlightId &&
                b.Passenger.Email.Value.Equals(modifyBookingRequest.PassengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
                return false;

            booking.FlightId = modifyBookingRequest.NewFlightId;
            booking.SelectedClass = modifyBookingRequest.NewClass;
            booking.PricePaid = flight.GetPriceForClass(modifyBookingRequest.NewClass);
            
            _bookingService.Update(booking);
            return true;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail)
        {
            return _bookingService.GetAll()
             .Where(b => b.Passenger.Email.Value.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase))
             .ToList();
        }
    }
}