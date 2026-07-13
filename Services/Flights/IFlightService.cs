using TASK2.Models;

namespace TASK2.Services.Flights
{
    public interface IFlightService
    {
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
        public IReadOnlyCollection<Flight> GetAll();
        public IReadOnlyCollection<Flight> Add(ICollection<Flight> flights);
        public Flight Update(Flight flight);
        public void Delete(int id);
    }
}