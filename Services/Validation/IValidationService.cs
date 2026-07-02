using TASK2.Models;

namespace TASK2.Services.Validation
{
    public interface IValidationService
    {
        public (bool IsValid, IReadOnlyCollection<FileValidationError> Errors, Flight? ValidFlight) ValidateFlightRow(
            string csvLine,
            int rowNumber,
            IReadOnlyCollection<Flight>? existingFlights = null);
            
    }
}
