using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using TASK2.File_Storage;
using TASK2.Models;
using TASK2.Validation;

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
            var importedFlightIds = new HashSet<int>();

            if (!System.IO.File.Exists(filePath))
            {
                errors.Add(new ValidationError
                {
                    RowNumber = 0,
                    FieldName = "File",
                    ErrorMessage = "The specified file does not exist."
                });

                return (false, errors);
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            var existingFlights = _flightRepo.GetAll();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var (isValid, rowErrors, validFlight) = _validationService.ValidateFlightRow(lines[i], i + 1, existingFlights);

                if (!isValid)
                {
                    errors.AddRange(rowErrors);
                }
                else if (validFlight != null)
                {
                    if (!importedFlightIds.Add(validFlight.Id))
                    {
                        AddDuplicateFlightIdError(errors, i + 1, validFlight.Id);
                        continue;
                    }

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
                if (flight == null)
                    return false;

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
            return typeof(Flight)
                .GetProperties()
                .Select(property => new FieldValidationInfo
                {
                    FieldName = property.Name,
                    Type = GetReadableType(property.PropertyType),
                    Constraints = GetReadableConstraints(property)
                })
                .ToList();
        }

        public List<Flight> GetAllFlights()
        {
            return _flightRepo.GetAll();
        }

        public List<ValidationError> ValidateImportedFlightData(string filePath)
        {
            var errors = new List<ValidationError>();
            var importedFlightIds = new HashSet<int>();

            if (!System.IO.File.Exists(filePath))
            {
                errors.Add(new ValidationError
                {
                    RowNumber = 0,
                    FieldName = "File",
                    ErrorMessage = "The specified file does not exist."
                });

                return errors;
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            var existingFlights = _flightRepo.GetAll();

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var result = _validationService.ValidateFlightRow(lines[i], i + 1, existingFlights);

                if (!result.IsValid)
                {
                    errors.AddRange(result.Errors);
                }
                else if (result.ValidFlight != null && !importedFlightIds.Add(result.ValidFlight.Id))
                {
                    AddDuplicateFlightIdError(errors, i + 1, result.ValidFlight.Id);
                }
            }

            return errors;
        }

        private static void AddDuplicateFlightIdError(List<ValidationError> errors, int rowNumber, int flightId)
        {
            errors.Add(new ValidationError
            {
                RowNumber = rowNumber,
                FieldName = "Id",
                ErrorMessage = $"Flight ID {flightId} is duplicated in the imported file."
            });
        }

        private static string GetReadableType(Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type) ?? type;

            if (actualType == typeof(string))
                return "Free Text";

            if (actualType == typeof(DateTime))
                return "Date Time";

            if (actualType == typeof(decimal))
                return "Decimal";

            if (actualType == typeof(int))
                return "Integer";

            return actualType.Name;
        }

        private static string GetReadableConstraints(PropertyInfo property)
        {
            var constraints = new List<string>();
            var attributes = property.GetCustomAttributes<ValidationAttribute>().ToList();

            if (attributes.OfType<RequiredAttribute>().Any())
                constraints.Add("Required");

            foreach (var rangeAttribute in attributes.OfType<RangeAttribute>())
            {
                constraints.Add($"Allowed Range ({FormatRangeValue(rangeAttribute.Minimum)} -> {FormatRangeValue(rangeAttribute.Maximum)})");
            }

            if (attributes.OfType<FutureOrTodayAttribute>().Any())
                constraints.Add("Allowed Range (Today -> Future)");

            if (property.Name == nameof(Flight.Id))
                constraints.Add("Unique");

            return constraints.Count == 0 ? "None" : string.Join(", ", constraints);
        }

        private static string FormatRangeValue(object value)
        {
            return value switch
            {
                int intValue when intValue == int.MaxValue => "Max",
                decimal decimalValue when decimalValue == decimal.MaxValue => "Max",
                _ => value.ToString() ?? string.Empty
            };
        }
    }
}
