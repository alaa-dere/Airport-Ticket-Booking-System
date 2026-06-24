using System;
using System.Collections.Generic;
using System.Linq;
using TASK2.File_Storage;
using TASK2.Models;

namespace TASK2.Services
{
    public class ManagerService
    {
        private readonly FlightRepository _flightRepo;
        private readonly BookingRepository _bookingRepo;
        private readonly ValidationService _validationService; 

        public ManagerService()
        {
            _flightRepo = new FlightRepository();
            _bookingRepo = new BookingRepository();
            _validationService = new ValidationService(); 
        }
public (bool IsSuccess, List<ValidationError> Errors) BatchUploadFlights(string filePath)
        {
            var errors = new List<ValidationError>();
            var validFlights = new List<Flight>();

            if (!System.IO.File.Exists(filePath))
            {
                errors.Add(new ValidationError { RowNumber = 0, FieldName = "File", ErrorMessage = "The specified file does not exist." });
                return (false, errors);
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                var (isValid, rowErrors, validFlight) = _validationService.ValidateFlightRow(lines[i], i + 1);

                if (!isValid)
                {
                    errors.AddRange(rowErrors);
                }
                else if (validFlight != null)
                {
                    validFlights.Add(validFlight);
                }
            }

            if (errors.Count > 0)
            {
                return (false, errors);
            }

            _flightRepo.AddFlights(validFlights);
            return (true, errors);
        }
        public List<Booking> FilterBookings(
            int? flightId = null,
            decimal? maxPrice = null,
            string? departureCountry = null,
            string? destinationCountry = null,
            DateTime? departureDate = null,
            string? departureAirport = null,
            string? arrivalAirport = null,
            string? passengerEmail = null,
            FlightClass? flightClass = null)
        {
            var allBookings = _bookingRepo.GetAll();
            var allFlights = _flightRepo.GetAll();

            return allBookings.Where(b =>
            {
                var flight = allFlights.FirstOrDefault(f => f.Id == b.FlightId);
                if (flight == null) return false;

                    return (!flightId.HasValue || b.FlightId == flightId.Value) &&
                       (!maxPrice.HasValue || b.PricePaid <= maxPrice.Value) &&
                       (string.IsNullOrEmpty(passengerEmail) || b.PassengerEmail!.Equals(passengerEmail, StringComparison.OrdinalIgnoreCase)) &&
                       (!flightClass.HasValue || b.SelectedClass == flightClass.Value) &&
                       (string.IsNullOrEmpty(departureCountry) || flight.DepartureCountry!.Equals(departureCountry, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(destinationCountry) || flight.DestinationCountry!.Equals(destinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(departureAirport) || flight.DepartureAirport!.Equals(departureAirport, StringComparison.OrdinalIgnoreCase)) &&
                       (string.IsNullOrEmpty(arrivalAirport) || flight.ArrivalAirport!.Equals(arrivalAirport, StringComparison.OrdinalIgnoreCase)) &&
                       (!departureDate.HasValue || flight.DepartureTime.Date == departureDate.Value.Date);
                     }).ToList();
        }
        public List<FieldValidationInfo> GetFlightValidationDetails()
        {
            return new List<FieldValidationInfo>
            {
                new FieldValidationInfo { FieldName = "Id", Type = "Integer", Constraints = "Required, Unique, Must be a positive number" },
                new FieldValidationInfo { FieldName = "DepartureCountry", Type = "Free Text", Constraints = "Required" },
                new FieldValidationInfo { FieldName = "DestinationCountry", Type = "Free Text", Constraints = "Required" },
                new FieldValidationInfo { FieldName = "DepartureTime", Type = "Date Time (YYYY-MM-DD)", Constraints = "Required, Allowed Range (Today -> Future)" },
                new FieldValidationInfo { FieldName = "DepartureAirport", Type = "Free Text", Constraints = "Required" },
                new FieldValidationInfo { FieldName = "ArrivalAirport", Type = "Free Text", Constraints = "Required" },
                new FieldValidationInfo { FieldName = "BasePrice", Type = "Decimal", Constraints = "Required, Must be a positive number >= 0" }
            };
        }

    }
}