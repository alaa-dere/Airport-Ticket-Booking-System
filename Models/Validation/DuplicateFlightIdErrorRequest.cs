namespace TASK2.Models;

public class DuplicateFlightIdErrorRequest
{
    public required ICollection<FileValidationError> Errors { get; set; }
    public int RowNumber { get; set; }
    public int FlightId { get; set; }
}
