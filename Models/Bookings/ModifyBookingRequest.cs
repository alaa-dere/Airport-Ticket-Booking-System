namespace TASK2.Models;

public class ModifyBookingRequest
{
    public int BookingId { get; set; }
    public required string PassengerEmail { get; set; }
    public int NewFlightId { get; set; }
    public FlightClass NewClass { get; set; }
}
