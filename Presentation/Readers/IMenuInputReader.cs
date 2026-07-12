using TASK2.Models;

namespace TASK2.Presentation.Readers
{
    public interface IMenuInputReader
    {
        public BookingFilter ReadBookingFilter();
        public FlightFilter ReadFlightFilter();
        public string ReadRequiredSimpleText(string message);
        public bool TryReadFlightIdFromSearchResults(
            IReadOnlyCollection<Flight> flights,
            string prompt,
            out int flightId
            );
        public bool TryReadBookingIdToModify(out int bookingId);
        public bool TryReadRequiredFlightClass(string heading, out FlightClass flightClass);
    }
}