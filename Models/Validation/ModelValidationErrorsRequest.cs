namespace TASK2.Models;

public class ModelValidationErrorsRequest
{
    public required Flight Flight { get; set; }
    public int RowNumber { get; set; }
    public required ICollection<FileValidationError> Errors { get; set; }
}
