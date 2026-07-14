using TASK2.Models;
public class FlightRowValidationResult
{
    public bool IsValid { get; set; }
    public IReadOnlyCollection<FileValidationError> Errors { get; set; } = [];
    public Flight? ValidFlight { get; set; }
}