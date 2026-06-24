using System;
using System.Collections.Generic;
using System.Linq;
using TASK2.File_Storage;
using TASK2.Models;

namespace TASK2.Services
{
    public class ManagerService
    {
        private readonly FlightRepository _flightRepo;
        private readonly BookingRepository _bookingRepo;

        public ManagerService()
        {
            _flightRepo = new FlightRepository();
            _bookingRepo = new BookingRepository();
        }

        public List<Booking> FilterBookings(
            int? flightId = null,
            decimal? maxPrice = null,
            string? departureCountry = null,
            string? destinationCountry = null,
            DateTime? departureDate = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            string? passengerEmail = null,
            FlightClass? flightClass = null)
        {
            var allBookings = _bookingRepo.GetAll();
            var allFlights = _flightRepo.GetAll();

            return allBookings.Where(b =>
            {
                var flight = allFlights.FirstOrDefault(f => f.Id == b.FlightId);
                if (flight == null) return false;

                return (!flightId.HasValue || b.FlightId == flightId.Value) &&
                       (!maxPrice.HasValue || b.PricePaid <= maxPrice.Value) &&
                       (string.IsNullOrEmpty(passengerEmail) || b.PassengerEmail.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase)) &&
                       (!flightClass.HasValue || b.SelectedClass == flightClass.Value) &&
                       (string.IsNullOrEmpty(departureCountry) || flight.DepartureCountry.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(destinationCountry) || flight.DestinationCountry.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(departureAirport) || flight.DepartureAirport.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(arrivalAirport) || flight.ArrivalAirport.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase)) &&
                       (!departureDate.HasValue || flight.DepartureTime.Date == departureDate.Value.Date);
            }).ToList();
        }
    }
}