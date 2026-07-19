using TASK2.Models;

namespace TASK2.Presentation.Readers
{
    public interface IMenuInputReader
    {
        /// <summary>
        /// Reads booking filter criteria from menu input.
        /// </summary>
        /// <returns>The booking filter entered by the user.</returns>
        public BookingFilter ReadBookingFilter();

        /// <summary>
        /// Reads flight filter criteria from menu input.
        /// </summary>
        /// <returns>The flight filter entered by the user.</returns>
        public FlightFilter ReadFlightFilter();

        /// <summary>
        /// Reads required simple text from the user.
        /// </summary>
        /// <param name="message">The prompt displayed to the user.</param>
        /// <returns>The validated text value.</returns>
        public string ReadRequiredSimpleText(string message);

        /// <summary>
        /// Reads a flight identifier and verifies it exists in the provided search results.
        /// </summary>
        /// <param name="request">The flight selection request details.</param>
        /// <param name="flightId">The selected flight identifier when reading succeeds.</param>
        /// <returns>True when a valid flight identifier is read; otherwise false.</returns>
        public bool TryReadFlightIdFromSearchResults(
            FlightIdSelectionRequest request,
            out int flightId
            );

        /// <summary>
        /// Reads the booking identifier to modify.
        /// </summary>
        /// <param name="bookingId">The booking identifier when reading succeeds.</param>
        /// <returns>True when a valid booking identifier is read; otherwise false.</returns>
        public bool TryReadBookingIdToModify(out int bookingId);

        /// <summary>
        /// Reads a required flight class selection.
        /// </summary>
        /// <param name="heading">The heading displayed before the class options.</param>
        /// <param name="flightClass">The selected flight class when reading succeeds.</param>
        /// <returns>True when a valid flight class is read; otherwise false.</returns>
        public bool TryReadRequiredFlightClass(string heading, out FlightClass flightClass);
    }
}