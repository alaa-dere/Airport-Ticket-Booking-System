using TASK2.Models;

namespace TASK2.Services.Validation
{
    public interface IValidationService
    {
        /// <summary>
        /// Validates a single CSV flight row and builds a flight when the row is valid.
        /// </summary>
        /// <param name="request">The flight row validation details.</param>
        /// <returns>The validation result, including errors and the valid flight when available.</returns>
        public FlightRowValidationResult ValidateFlightRow(ValidateFlightRowRequest request);
    }
}