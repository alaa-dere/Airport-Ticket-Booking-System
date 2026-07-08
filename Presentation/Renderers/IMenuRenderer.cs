using TASK2.Models;

namespace TASK2.Presentation.Renderers
{
    public interface IMenuRenderer
    {
        void DisplayFilteredBookings(IReadOnlyCollection<Booking> bookings);
        void DisplayPassengerBookings(IReadOnlyCollection<Booking> bookings);
        void DisplayPassengerFlights(IReadOnlyCollection<Flight> flights);
        void DisplayManagerFlights(IReadOnlyCollection<Flight> flights);
        void DisplayValidationErrors(IReadOnlyCollection<FileValidationError> errors);
        void DisplayValidationGuide(IReadOnlyCollection<FieldValidationInfo> guide);
        void WaitForReturn();
    }
}