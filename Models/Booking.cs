namespace TASK2.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public required string PassengerEmail { get; set; }
        public required string PassengerName { get; set; }
        public required string PassengerPhone { get; set; }
        public FlightClass SelectedClass { get; set; }
        public decimal PricePaid { get; set; }
    }
}