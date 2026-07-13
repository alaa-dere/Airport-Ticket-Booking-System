using TASK2.Models;

namespace TASK2.File_Storage.Flights
{
    public interface IFlightRepository
    {
        /// <summary>
        /// Searches flights based on the provided flight criteria.
        /// </summary>
        /// <param name="filter">The flight filter criteria.</param>
        /// <returns>The flights that match the filter criteria.</returns>
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);

        /// <summary>
        /// Gets all stored flights.
        /// </summary>
        /// <returns>All flights.</returns>
        public IReadOnlyCollection<Flight> GetAll();

        /// <summary>
        /// Adds new flights to storage.
        /// </summary>
        /// <param name="newFlights">The flights to add.</param>
        public void Add(ICollection<Flight> newFlights);

        /// <summary>
        /// Updates an existing flight in storage.
        /// </summary>
        /// <param name="updatedFlight">The flight data to save.</param>
        public void Update(Flight updatedFlight);

        /// <summary>
        /// Deletes a flight by its identifier.
        /// </summary>
        /// <param name="id">The flight identifier.</param>
        public void Delete(int id);

        /// <summary>
        /// Replaces all stored flights with the provided collection.
        /// </summary>
        /// <param name="flights">The flights to persist.</param>
        public void SaveAll(ICollection<Flight> flights);
    }
}