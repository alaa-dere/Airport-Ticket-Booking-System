namespace TASK2.Models;

public class ValidateFlightRowRequest
{
    public required string CsvLine { get; set; }
    public int RowNumber { get; set; }
    public IReadOnlyCollection<Flight>? ExistingFlights { get; set; }
}
