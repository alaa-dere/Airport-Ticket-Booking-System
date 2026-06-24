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

        public PassengerService()
        {
            _flightRepo = new FlightRepository();
            _bookingRepo = new BookingRepository();
        }

        public List<Flight> SearchFlights(
            string? departureCountry, 
            string? destinationCountry, 
            DateTime? departureDate, 
            decimal? maxPrice)
        {
            var flights = _flightRepo.GetAll();

            var filteredFlights = flights.Where(f =>
                (string.IsNullOrEmpty(departureCountry) || f.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(destinationCountry) || f.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                (!departureDate.HasValue || f.DepartureTime.Date == departureDate.Value.Date) &&
                (!maxPrice.HasValue || f.BasePrice <= maxPrice.Value)
            ).ToList();

            return filteredFlights;
        }

        public bool BookFlight(int flightId, string passengerEmail, string passengerName, string passengerPhone, FlightClass selectedClass)
        {
            var flight = _flightRepo.GetAll().FirstOrDefault(f => f.Id == flightId);
            
            if (flight == null)
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

            _bookingRepo.AddBooking(newBooking);
            return true;
        }

    }

    
}