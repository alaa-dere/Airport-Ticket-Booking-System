using TASK2.Models;

namespace TASK2.Presentation.Renderers
{
    public interface IMenuRenderer
    {
        /// <summary>
        /// Displays filtered booking results for manager views.
        /// </summary>
        /// <param name="bookings">The bookings to display.</param>
        public void DisplayFilteredBookings(IReadOnlyCollection<Booking> bookings);

        /// <summary>
        /// Displays bookings for a passenger view.
        /// </summary>
        /// <param name="bookings">The bookings to display.</param>
        public void DisplayPassengerBookings(IReadOnlyCollection<Booking> bookings);

        /// <summary>
        /// Displays flight search results for a passenger view.
        /// </summary>
        /// <param name="flights">The flights to display.</param>
        public void DisplayPassengerFlights(IReadOnlyCollection<Flight> flights);

        /// <summary>
        /// Displays flights for a manager view.
        /// </summary>
        /// <param name="flights">The flights to display.</param>
        public void DisplayManagerFlights(IReadOnlyCollection<Flight> flights);

        /// <summary>
        /// Displays file validation errors.
        /// </summary>
        /// <param name="errors">The validation errors to display.</param>
        public void DisplayValidationErrors(IReadOnlyCollection<FileValidationError> errors);

        /// <summary>
        /// Displays the flight validation guide.
        /// </summary>
        /// <param name="guide">The validation guide to display.</param>
        public void DisplayValidationGuide(IReadOnlyCollection<FieldValidationInfo> guide);

        /// <summary>
        /// Waits for the user to return to the previous menu.
        /// </summary>
        public void WaitForReturn();
    }
}
