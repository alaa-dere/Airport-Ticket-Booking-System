using TASK2.Models;
namespace TASK2.Services.Passengers
{
    public interface IPassengerService
    {
        public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
        public bool Book(int flightId, string passengerEmail, string passengerName, string passengerPhone, FlightClass selectedClass);
        public bool Cancel(int bookingId, string passengerEmail);
        public bool Modify(int bookingId, string passengerEmail, int newFlightId, FlightClass newClass);
        public IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail);
    }
}