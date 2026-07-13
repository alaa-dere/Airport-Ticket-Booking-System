using TASK2.Models;

namespace TASK2.Services.Validation
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates a single CSV flight row and builds a flight when the row is valid.
        /// </summary>
        /// <param name="csvLine">The CSV line to validate.</param>
        /// <param name="rowNumber">The source file row number.</param>
        /// <param name="existingFlights">The existing flights used to check duplicate identifiers.</param>
        /// <returns>The validation result, including errors and the valid flight when available.</returns>
        public FlightRowValidationResult ValidateFlightRow(
            string csvLine,
            int rowNumber,
            IReadOnlyCollection<Flight>? existingFlights = null
            );
    }
}