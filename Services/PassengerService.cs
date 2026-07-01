using System;
using System.Collections.Generic;
using System.Linq; 
using TASK2.File_Storage; 
using TASK2.Models;

namespace TASK2.Services
{
    public class PassengerService
    {
        private readonly FlightRepository _flightRepo;
        private readonly BookingRepository _bookingRepo;
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);

        public PassengerService()
        {
            _flightRepo = new FlightRepository();
            _bookingRepo = new BookingRepository();
        }

        public List<Flight> SearchFlights(
            string? departureCountry = null, 
            string? destinationCountry = null, 
            DateTime? departureDate = null, 
            decimal? maxPrice = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            FlightClass? flightClass = null)
        {
            var flights = _flightRepo.GetAll();

            return flights.Where(f =>
                (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry?.Equals(departureCountry, StringComparison.OrdinalIgnoreCase) == true) &&
                (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry?.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase) == true) &&
                (string.IsNullOrEmpty(departureAirport) || f.DepartureAirport?.Equals(departureAirport, StringComparison.OrdinalIgnoreCase) == true) &&
                (string.IsNullOrEmpty(arrivalAirport) || f.ArrivalAirport?.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase) == true) &&
                (!departureDate.HasValue || f.DepartureTime.Date == departureDate.Value.Date) &&
                (!maxPrice.HasValue || f.GetPriceForClass(flightClass ?? FlightClass.Economy) <= maxPrice.Value)
            ).ToList();
        }

        public bool BookFlight(int flightId, string passengerEmail, string passengerName, string passengerPhone, FlightClass selectedClass)
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

            var flight = _flightRepo.GetAll().FirstOrDefault(f => f.Id == flightId);
            
            if (flight == null)
            {
                return false;
            }

            var alreadyBooked = _bookingRepo.GetAll().Any(b =>
                b.FlightId == flightId &&
                b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
            {
                return false;
            }

            decimal finalPrice = flight.GetPriceForClass(selectedClass);

            var newBooking = new Booking
            {
                FlightId = flightId,
                PassengerEmail = passengerEmail,
                PassengerName = passengerName,
                PassengerPhone = passengerPhone,
                SelectedClass = selectedClass,
                PricePaid = finalPrice
            };

            _bookingRepo.Add(newBooking);
            return true;
        }
        public bool CancelBooking(int bookingId, string passengerEmail)
        {
        var booking = _bookingRepo.GetAll().FirstOrDefault(b => b.Id == bookingId && b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));
            
            if (booking == null)
                return false;

            _bookingRepo.Delete(bookingId);
            return true;
        }
       
        public bool ModifyBooking(int bookingId, string passengerEmail, int newFlightId, FlightClass newClass)
        {
        var booking = _bookingRepo.GetAll().FirstOrDefault(b => b.Id == bookingId && b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));
            
            if (booking == null)
                return false;

            var flight = _flightRepo.GetAll().FirstOrDefault(f => f.Id == newFlightId);
            
            if (flight == null)
                return false;

            var alreadyBooked = _bookingRepo.GetAll().Any(b =>
                b.Id != bookingId &&
                b.FlightId == newFlightId &&
                b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase));

            if (alreadyBooked)
                return false;

            booking.FlightId = newFlightId;
            booking.SelectedClass = newClass;
            booking.PricePaid = flight.GetPriceForClass(newClass);
            
            _bookingRepo.Update(booking);
            return true;
        }

        public List<Booking> GetMyBookings(string passengerEmail)
        {
            return _bookingRepo.GetAll()
             .Where(b => b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase))
             .ToList();
        }
    }

    
}
