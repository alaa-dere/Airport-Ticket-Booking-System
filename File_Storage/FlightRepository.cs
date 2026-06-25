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

            var columns = line.Split(',');
            if (columns.Length == 7)
            {
                var flight = new Flight
                {
                    Id = int.Parse(columns[0]),
                    DepartureCountry = columns[1],
                    DestinationCountry = columns[2],
                    DepartureAirport = columns[3],
                    ArrivalAirport = columns[4],
                    DepartureTime = DateTime.ParseExact(columns[5], "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    BasePrice = decimal.Parse(columns[6], CultureInfo.InvariantCulture)
                };
                flights.Add(flight);
            }
        }
        return flights;
    }

    public void AddFlights(List<Flight> newFlights)
    {
        var flights = GetAll();
        foreach (var flight in newFlights)
        {
            var maxId = flights.Count > 0 ? flights.Max(f => f.Id) : 0;
            flight.Id = maxId + 1;
            flights.Add(flight);
        }
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
        
        lines.AddRange(flights.Select(f => $"{f.Id},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureAirport},{f.ArrivalAirport},{f.DepartureTime:yyyy-MM-dd HH:mm},{f.BasePrice.ToString(CultureInfo.InvariantCulture)}"));
        
        File.WriteAllLines(FilePath, lines);
    }
}