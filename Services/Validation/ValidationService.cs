using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using TASK2.File_Storage.Flights;
using TASK2.File_Storage.Parser;
using TASK2.Models;

namespace TASK2.Services.Validation;
    public class ValidationService : IValidationService
    {
        private readonly IFlightRepository _flightRepo;
        private static readonly IParser Parser = ParserFactory.GetParser(ParserFactory.CsvParserType);

        public ValidationService(IFlightRepository flightRepo)
        {
            _flightRepo = flightRepo;
        }

        /// <inheritdoc />
        public FlightRowValidationResult ValidateFlightRow(ValidateFlightRowRequest request)
        {
            var errors = new List<FileValidationError>();

            if (string.IsNullOrWhiteSpace(request.CsvLine))
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "Row",
                    ErrorMessage = "Row is empty."
                });

                return new FlightRowValidationResult
                {
                    IsValid = false,
                    Errors = errors,
                    ValidFlight = null
                };
            }

            var columns = Parser.ParseLine(request.CsvLine);

            if (columns.Length != 7)
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "Row",
                    ErrorMessage = "Invalid row format. Expected 7 columns."
                });

                return new FlightRowValidationResult
                {
                    IsValid = false,
                    Errors = errors,
                    ValidFlight = null
                };
            }

            string idStr = columns[0].Trim();
            string departureCountry = columns[1].Trim();
            string destinationCountry = columns[2].Trim();
            string departureAirport = columns[3].Trim();
            string arrivalAirport = columns[4].Trim();
            string departureTimeStr = columns[5].Trim();
            string basePriceStr = columns[6].Trim();

            int id = 0;

            if (!int.TryParse(idStr, out id) || id <= 0)
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "Id",
                    ErrorMessage = "Flight ID must be a valid positive integer."
                });
            }
            else if ((request.ExistingFlights ?? _flightRepo.GetAll()).Any(f => f.Id == id))
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "Id",
                    ErrorMessage = $"Flight ID {id} already exists in the system."
                });
            }

            DateTime departureTime = DateTime.MinValue;

            if (!DateTime.TryParseExact(
                    departureTimeStr,
                    "yyyy-MM-dd HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out departureTime))
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "DepartureTime",
                    ErrorMessage = "Departure Time must be in format yyyy-MM-dd HH:mm."
                });
            }

            decimal basePrice = 0;

            if (!decimal.TryParse(
                    basePriceStr,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out basePrice) || basePrice < 0)
            {
                errors.Add(new FileValidationError
                {
                    RowNumber = request.RowNumber,
                    FieldName = "BasePrice",
                    ErrorMessage = "Base Price must be a valid positive number."
                });
            }

            var flight = new Flight
            {
                Id = id,
                DepartureCountry = string.IsNullOrWhiteSpace(departureCountry) ? null : departureCountry,
                DestinationCountry = string.IsNullOrWhiteSpace(destinationCountry) ? null : destinationCountry,
                DepartureAirport = string.IsNullOrWhiteSpace(departureAirport) ? null : departureAirport,
                ArrivalAirport = string.IsNullOrWhiteSpace(arrivalAirport) ? null : arrivalAirport,
                DepartureTime = departureTime,
                BasePrice = basePrice
            };

            AddModelValidationErrors(new ModelValidationErrorsRequest
            {
                Flight = flight,
                RowNumber = request.RowNumber,
                Errors = errors
            });

            if (errors.Count > 0)
            {
                return new FlightRowValidationResult
                {
                    IsValid = false,
                    Errors = errors,
                    ValidFlight = null
                };
            }

            return new FlightRowValidationResult
            {
                IsValid = true,
                Errors = errors,
                ValidFlight = flight
            };
        }

        private static void AddModelValidationErrors(ModelValidationErrorsRequest request)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request.Flight);

            Validator.TryValidateObject(request.Flight, validationContext, validationResults, true);

            foreach (var result in validationResults)
            {
                var fieldNames = result.MemberNames.Any()
                    ? result.MemberNames
                    : new[] { "Flight" };

                foreach (var fieldName in fieldNames)
                {
                    if (request.Errors.Any(e => e.FieldName == fieldName && e.ErrorMessage == result.ErrorMessage))
                    {
                        continue;
                    }

                    request.Errors.Add(new FileValidationError
                    {
                        RowNumber = request.RowNumber,
                        FieldName = fieldName,
                        ErrorMessage = result.ErrorMessage ?? "Invalid value."
                    });
                }
            }
        }
    }