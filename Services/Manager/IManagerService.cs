using TASK2.Models;

namespace TASK2.Services.Manager
{
    public interface IManagerService
    {
        /// <summary>
        /// Validates and imports flights from a file.
        /// </summary>
        /// <param name="filePath">The path of the file to import.</param>
        /// <returns>The upload result and any validation errors.</returns>
        public (bool IsSuccess, IReadOnlyCollection<FileValidationError> Errors) BatchUploadFlights(string filePath);

        /// <summary>
        /// Filters bookings based on the provided booking criteria.
        /// </summary>
        /// <param name="filter">The booking filter criteria.</param>
        /// <returns>The bookings that match the filter criteria.</returns>
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);

        /// <summary>
        /// Gets validation details for flight fields.
        /// </summary>
        /// <returns>The flight field validation details.</returns>
        public IReadOnlyCollection<FieldValidationInfo> GetFlightValidationDetails();

        /// <summary>
        /// Gets all flights.
        /// </summary>
        /// <returns>All flights.</returns>
        public IReadOnlyCollection<Flight> GetAll();

        /// <summary>
        /// Validates imported flight data without saving it.
        /// </summary>
        /// <param name="filePath">The path of the file to validate.</param>
        /// <returns>The validation errors found in the file.</returns>
        public IReadOnlyCollection<FileValidationError> ValidateImportedFlightData(string filePath);           
    }
}