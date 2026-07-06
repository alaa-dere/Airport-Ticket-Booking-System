namespace TASK2.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        public required Passenger Passenger { get; set; }
        public FlightClass SelectedClass { get; set; }
        public decimal PricePaid { get; set; }
    }
}