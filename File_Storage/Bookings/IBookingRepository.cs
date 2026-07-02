using TASK2.Models;

namespace TASK2.File_Storage.Bookings;
public interface IBookingRepository
{
    IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);
    public IReadOnlyCollection<Booking> GetAll();
    public void Add(Booking booking);
    public void Update(Booking updatedBooking);
    public void Delete(int id);
    public void SaveAll(ICollection<Booking> bookings);
}
