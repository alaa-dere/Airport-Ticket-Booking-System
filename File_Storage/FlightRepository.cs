using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.Models;

namespace TASK2.File_Storage;

public class FlightRepository
{
    private const string FilePath = "flights_storage.csv";

    public List<Flight> GetAll()
    {
        var flights = new List<Flight>();

        if (!File.Exists(FilePath) || File.ReadLines(FilePath).Count() <= 1)
            return flights;

        var lines = File.ReadAllLines(FilePath);
        lines = lines.Skip(1).ToArray();
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = CsvUtility.ParseLine(line);

            if (columns.Length == 7 &&
                int.TryParse(columns[0], out var id) &&
                DateTime.TryParseExact(columns[5], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var departureTime) &&
                decimal.TryParse(columns[6], NumberStyles.Number, CultureInfo.InvariantCulture, out var basePrice))
            {
                var flight = new Flight
                {
                    Id = id,
                    DepartureCountry = columns[1],
                    DestinationCountry = columns[2],
                    DepartureAirport = columns[3],
                    ArrivalAirport = columns[4],
                    DepartureTime = departureTime,
                    BasePrice = basePrice
                };
                flights.Add(flight);
            }
        }
        return flights;
    }

    public void AddFlights(List<Flight> newFlights)
    {
        var flights = GetAll();
        flights.AddRange(newFlights);
        SaveAll(flights);
    }

    public void UpdateFlight(Flight updatedFlight)
    {
        var flights = GetAll();
        var existingFlight = flights.FirstOrDefault(f => f.Id == updatedFlight.Id);
        if (existingFlight != null)
        {
            existingFlight.DepartureCountry = updatedFlight.DepartureCountry;
            existingFlight.DestinationCountry = updatedFlight.DestinationCountry;
            existingFlight.DepartureAirport = updatedFlight.DepartureAirport;
            existingFlight.ArrivalAirport = updatedFlight.ArrivalAirport;
            existingFlight.DepartureTime = updatedFlight.DepartureTime;
            existingFlight.BasePrice = updatedFlight.BasePrice;
            SaveAll(flights);
        }
    }

    public void DeleteFlight(int id)
    {
        var flights = GetAll();
        var flightToDelete = flights.FirstOrDefault(f => f.Id == id);
        if (flightToDelete != null)
        {
            flights.Remove(flightToDelete);
            SaveAll(flights);
        }
    }

    public void SaveAll(List<Flight> flights)
    {
        var lines = new List<string> { "Id,DepartureCountry,DestinationCountry,DepartureAirport,ArrivalAirport,DepartureTime,Price" };
        
        lines.AddRange(flights.Select(f => CsvUtility.ToLine(
            f.Id,
            f.DepartureCountry,
            f.DestinationCountry,
            f.DepartureAirport,
            f.ArrivalAirport,
            f.DepartureTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            f.BasePrice.ToString(CultureInfo.InvariantCulture))));
        
        File.WriteAllLines(FilePath, lines);
    }
}
