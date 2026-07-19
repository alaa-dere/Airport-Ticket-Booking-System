using System.ComponentModel.DataAnnotations;
using TASK2.File_Storage.Flights;
using TASK2.Models;

namespace TASK2.Services.Flights
{
    public class FlightService : IFlightService
    {
        private readonly IFlightRepository _flightRepository;

        public FlightService(IFlightRepository flightRepository)
        {
            _flightRepository = flightRepository;
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Flight> GetAll()
        {
            return _flightRepository.GetAll();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<Flight> Add(ICollection<Flight> flights)
        {
            ValidateFlightsForAdd(flights);
            _flightRepository.Add(flights);
            return flights.ToList();
        }

        /// <inheritdoc />
        public Flight Update(Flight flight)
        {
            ValidateFlight(flight);
            _flightRepository.Update(flight);
            return flight;
        }

        /// <inheritdoc />
        public void Delete(int id)
        {
            _flightRepository.Delete(id);
        }
        /// <inheritdoc />
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter)
        {
            return _flightRepository.SearchFlights(filter);
        }

        private void ValidateFlightsForAdd(ICollection<Flight> flights)
        {
            if (flights == null || flights.Count == 0)
                throw new ValidationException("At least one flight is required.");

            foreach (var flight in flights)
            {
                ValidateFlight(flight);
            }

            var duplicateNewFlightId = flights
                .GroupBy(f => f.Id)
                .FirstOrDefault(group => group.Count() > 1)
                ?.Key;

            if (duplicateNewFlightId.HasValue)
                throw new ValidationException($"Flight ID {duplicateNewFlightId.Value} is duplicated in the provided flights.");

            var existingFlightId = flights
                .Select(f => f.Id)
                .FirstOrDefault(id => _flightRepository.GetAll().Any(existingFlight => existingFlight.Id == id));

            if (existingFlightId > 0)
                throw new ValidationException($"Flight ID {existingFlightId} already exists in the system.");
        }

        private static void ValidateFlight(Flight flight)
        {
            if (flight == null)
                throw new ValidationException("Flight is required.");

            Validator.ValidateObject(flight, new ValidationContext(flight), true);
        }
    }
}