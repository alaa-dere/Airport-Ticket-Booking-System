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
    }
}