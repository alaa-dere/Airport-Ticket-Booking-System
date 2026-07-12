using TASK2.Models;

namespace TASK2.Services.Validation
{
    public interface IValidationService
    {
        public FlightRowValidationResult ValidateFlightRow(
            string csvLine,
            int rowNumber,
            IReadOnlyCollection<Flight>? existingFlights = null
            );
    }
}