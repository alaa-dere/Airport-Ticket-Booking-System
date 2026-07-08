using TASK2.File_Storage.Bookings;
using TASK2.Models;

namespace TASK2.Services.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public IReadOnlyCollection<Booking> GetAll()
        {
            return _bookingRepository.GetAll();
        }

        public void Add(Booking booking)
        {
            _bookingRepository.Add(booking);
        }

        public void Update(Booking booking)
        {
            _bookingRepository.Update(booking);
        }

        public void Delete(int id)
        {
            _bookingRepository.Delete(id);
        }
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
        {
            return _bookingRepository.FilterBookings(filter);
        }
    }
}