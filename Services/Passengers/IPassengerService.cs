using TASK2.Models;
namespace TASK2.Services.Passengers
{
    public interface IPassengerService
    {
        /// <summary>
        /// Searches available flights based on passenger criteria.
        /// </summary>
        /// <param name="filter">The flight filter criteria.</param>
        /// <returns>The flights that match the filter criteria.</returns>
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);

        /// <summary>
        /// Creates a booking for a passenger.
        /// </summary>
        /// <param name="bookingRequest">The booking request details.</param>
        /// <returns>The created booking.</returns>
        public Booking Book(BookingRequest bookingRequest);

        /// <summary>
        /// Cancels a passenger booking.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <param name="passengerEmail">The passenger email address.</param>
        public void Cancel(int bookingId, string passengerEmail);

        /// <summary>
        /// Modifies an existing passenger booking.
        /// </summary>
        /// <param name="modifyBookingRequest">The booking modification details.</param>
        /// <returns>True when the booking is modified; otherwise false.</returns>
        public bool Modify(ModifyBookingRequest modifyBookingRequest);

        /// <summary>
        /// Gets all bookings for a passenger.
        /// </summary>
        /// <param name="passengerEmail">The passenger email address.</param>
        /// <returns>The passenger's bookings.</returns>
        public IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail);
    }
}