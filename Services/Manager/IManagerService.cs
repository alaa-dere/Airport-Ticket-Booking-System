using TASK2.Models;

namespace TASK2.Services.Manager
{
    public interface IManagerService
    {
        public (bool IsSuccess, IReadOnlyCollection<FileValidationError> Errors) BatchUploadFlights(string filePath);
        public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);
        public IReadOnlyCollection<FieldValidationInfo> GetFlightValidationDetails();
        public IReadOnlyCollection<Flight> GetAll();
        public IReadOnlyCollection<FileValidationError> ValidateImportedFlightData(string filePath);           
    }
}