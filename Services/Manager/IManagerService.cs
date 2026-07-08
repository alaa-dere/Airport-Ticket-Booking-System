using TASK2.Models;

namespace TASK2.Services.Manager
{
    public interface IManagerService
    {
        (bool IsSuccess, IReadOnlyCollection<FileValidationError> Errors) BatchUploadFlights(string filePath);
        IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter);
        IReadOnlyCollection<FieldValidationInfo> GetFlightValidationDetails();
        IReadOnlyCollection<Flight> GetAll();
        IReadOnlyCollection<FileValidationError> ValidateImportedFlightData(string filePath);
            
    }
}