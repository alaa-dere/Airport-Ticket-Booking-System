using TASK2.Models;

namespace TASK2.Presentation.Readers
{
    public interface IMenuInputReader
    {
        BookingFilter ReadBookingFilter();
        FlightFilter ReadFlightFilter();
        string ReadRequiredSimpleText(string message);
        bool TryReadFlightIdFromSearchResults(
            IReadOnlyCollection<Flight> flights,
            string prompt,
            out int flightId);
        bool TryReadBookingIdToModify(out int bookingId);
        bool TryReadRequiredFlightClass(string heading, out FlightClass flightClass);
    }
}