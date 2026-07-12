using TASK2.Models;
namespace TASK2.Presentation.Readers
{
    public interface IConsoleReader
    {
       public string? ReadOptionalText();
        public int? ReadOptionalInt();
        public decimal? ReadOptionalDecimal();
        public DateTime? ReadOptionalDate();
        public FlightClass? ReadOptionalFlightClass();
    }
}