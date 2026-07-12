using TASK2.Models;

namespace TASK2.Presentation.Renderers
{
    public interface IMenuRenderer
    {
        public void DisplayFilteredBookings(IReadOnlyCollection<Booking> bookings);
        public void DisplayPassengerBookings(IReadOnlyCollection<Booking> bookings);
        public void DisplayPassengerFlights(IReadOnlyCollection<Flight> flights);
        public void DisplayManagerFlights(IReadOnlyCollection<Flight> flights);
        public void DisplayValidationErrors(IReadOnlyCollection<FileValidationError> errors);
        public void DisplayValidationGuide(IReadOnlyCollection<FieldValidationInfo> guide);
        public void WaitForReturn();
    }
}