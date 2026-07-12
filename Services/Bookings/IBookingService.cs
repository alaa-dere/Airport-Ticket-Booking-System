using TASK2.Models;

namespace TASK2.Services.Bookings
{
    public interface IBookingService
    {
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);
        public IReadOnlyCollection<Booking> GetAll();
        public void Add(Booking booking);
        public void Update(Booking booking);
        public void Delete(int id);
    }
}