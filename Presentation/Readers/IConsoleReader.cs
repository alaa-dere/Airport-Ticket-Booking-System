using TASK2.Models;
namespace TASK2.Presentation.Readers;
public interface IConsoleReader
{
        string? ReadOptionalText();
        int? ReadOptionalInt();
        decimal? ReadOptionalDecimal();
        DateTime? ReadOptionalDate();
        FlightClass? ReadOptionalFlightClass();
}