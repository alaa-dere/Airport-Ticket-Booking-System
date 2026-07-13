using TASK2.Models;

namespace TASK2.File_Storage.Bookings
{
    public interface IBookingRepository
    {
        /// <summary>
        /// Filters bookings based on the provided booking criteria.
        /// </summary>
        /// <param name="filter">The booking filter criteria.</param>
        /// <returns>The bookings that match the filter criteria.</returns>
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);

        /// <summary>
        /// Gets all stored bookings.
        /// </summary>
        /// <returns>All bookings.</returns>
        public IReadOnlyCollection<Booking> GetAll();

        /// <summary>
        /// Adds a new booking to storage.
        /// </summary>
        /// <param name="booking">The booking to add.</param>
        public void Add(Booking booking);

        /// <summary>
        /// Updates an existing booking in storage.
        /// </summary>
        /// <param name="updatedBooking">The booking data to save.</param>
        public void Update(Booking updatedBooking);

        /// <summary>
        /// Deletes a booking by its identifier.
        /// </summary>
        /// <param name="id">The booking identifier.</param>
        public void Delete(int id);

        /// <summary>
        /// Replaces all stored bookings with the provided collection.
        /// </summary>
        /// <param name="bookings">The bookings to persist.</param>
        public void SaveAll(ICollection<Booking> bookings);
    }
}