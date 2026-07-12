using TASK2.Models;
namespace TASK2.Services.Passengers
{
    public interface IPassengerService
    {
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
        public Booking Book(BookingRequest bookingRequest);
        public void Cancel(int bookingId, string passengerEmail);
        public bool Modify(ModifyBookingRequest modifyBookingRequest);
        public IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail);
    }
}