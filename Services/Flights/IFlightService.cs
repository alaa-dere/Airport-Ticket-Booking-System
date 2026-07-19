using TASK2.Models;

namespace TASK2.Services.Flights
{
    public interface IFlightService
    {
        /// <summary>
        /// Searches flights based on the provided flight criteria.
        /// </summary>
        /// <param name="filter">The flight filter criteria.</param>
        /// <returns>The flights that match the filter criteria.</returns>
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);

        /// <summary>
        /// Gets all flights.
        /// </summary>
        /// <returns>All flights.</returns>
        public IReadOnlyCollection<Flight> GetAll();

        /// <summary>
        /// Adds flights after applying flight validation.
        /// </summary>
        /// <param name="flights">The flights to add.</param>
        /// <returns>The added flights.</returns>
        public IReadOnlyCollection<Flight> Add(ICollection<Flight> flights);

        /// <summary>
        /// Updates a flight after applying flight validation.
        /// </summary>
        /// <param name="flight">The flight data to update.</param>
        /// <returns>The updated flight.</returns>
        public Flight Update(Flight flight);

        /// <summary>
        /// Deletes a flight by its identifier.
        /// </summary>
        /// <param name="id">The flight identifier.</param>
        public void Delete(int id);
    }
}