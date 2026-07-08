using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using TASK2.Services.Bookings;
using TASK2.Services.Flights;
using TASK2.Services.Validation;
using TASK2.Models;
using TASK2.Validation;

namespace TASK2.Services.Manager
{
    public class ManagerService : IManagerService
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly IValidationService _validationService;

        public ManagerService(
            IFlightService flightService,
            IBookingService bookingService,
            IValidationService validationService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _validationService = validationService;
        }

        public (bool IsSuccess, IReadOnlyCollection<FileValidationError> Errors) BatchUploadFlights(string filePath)
        {
            var errors = new List<FileValidationError>();
            var validFlights = new List<Flight>();
            var importedFlightIds = new HashSet<int>();

            if (!System.IO.File.Exists(filePath))
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = 0,
                    FieldName = "File",
                    ErrorMessage = "The specified file does not exist."
                });

                return (false, errors);
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            var existingFlights = _flightService.GetAll();

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

            _flightService.Add(validFlights);
            return (true, errors);
        }


        public IReadOnlyCollection<FieldValidationInfo> GetFlightValidationDetails()
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

        public IReadOnlyCollection<Flight> GetAll()
        {
            return _flightService.GetAll();
        }

        public IReadOnlyCollection<FileValidationError> ValidateImportedFlightData(string filePath)
        {
            var errors = new List<FileValidationError>();
            var importedFlightIds = new HashSet<int>();

            if (!System.IO.File.Exists(filePath))
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = 0,
                    FieldName = "File",
                    ErrorMessage = "The specified file does not exist."
                });

                return errors;
            }

            var lines = System.IO.File.ReadAllLines(filePath);
            var existingFlights = _flightService.GetAll();

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

        private static void AddDuplicateFlightIdError(ICollection<FileValidationError> errors, int rowNumber, int flightId)
        {
            errors.Add(new FileValidationError
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
         public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
        {
            return _bookingService.FilterBookings(filter);
        }
    }    
}