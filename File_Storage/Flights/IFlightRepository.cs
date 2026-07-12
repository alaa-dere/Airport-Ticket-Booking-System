using TASK2.Models;

namespace TASK2.File_Storage.Flights
{
    public interface IFlightRepository
    {
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
        public IReadOnlyCollection<Flight> GetAll();
        public void Add(ICollection<Flight> newFlights);
        public void Update(Flight updatedFlight);
        public void Delete(int id);
        public void SaveAll(ICollection<Flight> flights);
    }
}