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

        public IReadOnlyCollection<Flight> GetAll()
        {
            return _flightRepository.GetAll();
        }

        public void Add(ICollection<Flight> flights)
        {
            _flightRepository.Add(flights);
        }

        public void Update(Flight flight)
        {
            _flightRepository.Update(flight);
        }

        public void Delete(int id)
        {
            _flightRepository.Delete(id);
        }
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter)
        {
            return _flightRepository.SearchFlights(filter);
        }
    }
}