using System;
using System.Collections.Generic;
using System.Linq;
using TASK2.File_Storage;
using TASK2.Models;

namespace TASK2.Services
{
    public class ValidationService
    {
        private readonly FlightRepository _flightRepo;

        public ValidationService()
        {
            _flightRepo = new FlightRepository();
        }

        public (bool IsValid, List<ValidationError> Errors, Flight? ValidFlight) ValidateFlightRow(string csvLine, int rowNumber)
        {
            var errors = new List<ValidationError>();
            
            if (string.IsNullOrWhiteSpace(csvLine))
            {
                return (false, errors, null);
            }

            var columns = csvLine.Split(',');

            if (columns.Length < 7)
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "Row", ErrorMessage = "Invalid row format. Missing columns." });
                return (false, errors, null);
            }

            string idStr = columns[0].Trim();
            string departureCountry = columns[1].Trim();
            string destinationCountry = columns[2].Trim();
            string departureTimeStr = columns[4].Trim();
            string departureAirport = columns[3].Trim();
            string arrivalAirport = columns[5].Trim();
            string basePriceStr = columns[6].Trim();


            int id = 0;
            if (!int.TryParse(idStr, out id) || id <= 0)
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "Id", ErrorMessage = "Flight ID must be a valid positive integer." });
            }
            else if (_flightRepo.GetAll().Any(f => f.Id == id))
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "Id", ErrorMessage = $"Flight ID {id} already exists in the system." });
            }

            if (string.IsNullOrEmpty(departureCountry))
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "DepartureCountry", ErrorMessage = "Departure Country is required." });

            if (string.IsNullOrEmpty(destinationCountry))
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "DestinationCountry", ErrorMessage = "Destination Country is required." });

            if (string.IsNullOrEmpty(departureAirport))
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "DepartureAirport", ErrorMessage = "Departure Airport is required." });

            if (string.IsNullOrEmpty(arrivalAirport))
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "ArrivalAirport", ErrorMessage = "Arrival Airport is required." });

            DateTime departureTime = DateTime.MinValue;
            if (!DateTime.TryParse(departureTimeStr, out departureTime))
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "DepartureTime", ErrorMessage = "Departure Time must be a valid Date/Time format." });
            }
            else if (departureTime.Date < DateTime.Today)
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "DepartureTime", ErrorMessage = "Departure Time cannot be in the past. Must be today or future." });
            }

            decimal basePrice = 0;
            if (!decimal.TryParse(basePriceStr, out basePrice) || basePrice < 0)
            {
                errors.Add(new ValidationError { RowNumber = rowNumber, FieldName = "BasePrice", ErrorMessage = "Base Price must be a valid positive number." });
            }

            if (errors.Count > 0)
            {
                return (false, errors, null);
            }

            var flight = new Flight
            {
                Id = id,
                DepartureCountry = departureCountry,
                DestinationCountry = destinationCountry,
                DepartureTime = departureTime,
                DepartureAirport = departureAirport,
                ArrivalAirport = arrivalAirport,
                BasePrice = basePrice
            };

            return (true, errors, flight);
        }
    }
}