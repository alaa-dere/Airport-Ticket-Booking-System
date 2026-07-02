using TASK2.Models;

namespace TASK2.Services.Flights
{
    public interface IFlightService
    {
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
        IReadOnlyCollection<Flight> GetAll();
        void Add(ICollection<Flight> flights);
        void Update(Flight flight);
        void Delete(int id);
    }
}
