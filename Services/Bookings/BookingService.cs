using System.ComponentModel.DataAnnotations;
using TASK2.Extensions;
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

        public Booking Add(Booking booking)
        {
            ValidateBooking(booking, validateId: false);
            _bookingRepository.Add(booking);
            return booking;
        }

        public Booking Update(Booking booking)
        {
            ValidateBooking(booking, validateId: true);
            _bookingRepository.Update(booking);
            return booking;
        }

        public void Delete(int id)
        {
            _bookingRepository.Delete(id);
        }
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
        {
            return _bookingRepository.FilterBookings(filter);
        }

        private static void ValidateBooking(Booking booking, bool validateId)
        {
            if (booking == null)
                throw new ValidationException("Booking is required.");

            if (validateId && booking.Id <= 0)
                throw new ValidationException("Booking ID must be a valid positive integer.");

            if (booking.FlightId <= 0)
                throw new ValidationException("Flight ID must be a valid positive integer.");

            if (!Enum.IsDefined(typeof(FlightClass), booking.SelectedClass))
                throw new ValidationException("Selected class is invalid.");

            if (booking.PricePaid <= 0)
                throw new ValidationException("Price paid must be a valid positive number.");

            if (booking.Passenger == null ||
                string.IsNullOrWhiteSpace(booking.Passenger.Email) ||
                string.IsNullOrWhiteSpace(booking.Passenger.Name) ||
                string.IsNullOrWhiteSpace(booking.Passenger.Phone) ||
                !booking.Passenger.Email.IsValidSimpleValue() ||
                !booking.Passenger.Name.IsValidSimpleValue() ||
                !booking.Passenger.Phone.IsValidSimpleValue())
            {
                throw new ValidationException("Passenger email, name, and phone are required and must not contain commas or new lines.");
            }
        }
    }
}