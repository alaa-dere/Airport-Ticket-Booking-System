using TASK2.Models;

namespace TASK2.Services.Bookings
{
    public interface IBookingService
    {
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);
        IReadOnlyCollection<Booking> GetAll();
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(int id);
    }
}
