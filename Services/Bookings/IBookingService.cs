using TASK2.Models;

namespace TASK2.Services.Bookings
{
    public interface IBookingService
    {
        /// <summary>
        /// Filters bookings based on the provided booking criteria.
        /// </summary>
        /// <param name="filter">The booking filter criteria.</param>
        /// <returns>The bookings that match the filter criteria.</returns>
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);

        /// <summary>
        /// Gets all bookings.
        /// </summary>
        /// <returns>All bookings.</returns>
        public IReadOnlyCollection<Booking> GetAll();

        /// <summary>
        /// Adds a booking after applying booking validation.
        /// </summary>
        /// <param name="booking">The booking to add.</param>
        /// <returns>The added booking.</returns>
        public Booking Add(Booking booking);

        /// <summary>
        /// Updates a booking after applying booking validation.
        /// </summary>
        /// <param name="booking">The booking data to update.</param>
        /// <returns>The updated booking.</returns>
        public Booking Update(Booking booking);

        /// <summary>
        /// Deletes a booking by its identifier.
        /// </summary>
        /// <param name="id">The booking identifier.</param>
        public void Delete(int id);
    }
}