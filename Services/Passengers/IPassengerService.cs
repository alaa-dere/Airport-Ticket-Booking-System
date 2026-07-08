using TASK2.Models;
namespace TASK2.Services.Passengers
{
    public interface IPassengerService
    {
        IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter);
         bool Book(int flightId, string passengerEmail, string passengerName, string passengerPhone, FlightClass selectedClass);
         bool Cancel(int bookingId, string passengerEmail);
         bool Modify(int bookingId, string passengerEmail, int newFlightId, FlightClass newClass);
         IReadOnlyCollection<Booking> GetMyBookings(string passengerEmail);
    }
}