using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.File_Storage.Bookings;
using TASK2.Models;
using TASK2.File_Storage.Parser;

namespace TASK2.File_Storage.Flights;

public class FlightRepository : IFlightRepository, IBookingRepository
{
    private static readonly string FlightsFilePath = StoragePath.Resolve(AppConstants.FlightsFileName);
    private static readonly IParser FlightsParser = ParserFactory.GetParser(Path.GetExtension(FlightsFilePath).TrimStart('.'));
    private readonly IBookingRepository _bookingRepository;
    private readonly List<Flight> _flights;

    public FlightRepository(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
        _flights = LoadFlightsFromFile();
    }

    public IReadOnlyCollection<Flight> GetAll()
    {
        return _flights.ToList();
    }

    private List<Flight> LoadFlightsFromFile()
    {
        var flights = new List<Flight>();

        if (!File.Exists(FlightsFilePath) || File.ReadLines(FlightsFilePath).Count() <= 1)
            return flights;

        var lines = File.ReadAllLines(FlightsFilePath);
        lines = lines.Skip(1).ToArray();
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = FlightsParser.ParseLine(line);

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

    public void Add(ICollection<Flight> newFlights)
    {
        _flights.AddRange(newFlights);
        WriteFlightsToFile(_flights);
    }

    public void Update(Flight updatedFlight)
    {
        var existingFlight = _flights.FirstOrDefault(f => f.Id == updatedFlight.Id);
        if (existingFlight != null)
        {
            existingFlight.DepartureCountry = updatedFlight.DepartureCountry;
            existingFlight.DestinationCountry = updatedFlight.DestinationCountry;
            existingFlight.DepartureAirport = updatedFlight.DepartureAirport;
            existingFlight.ArrivalAirport = updatedFlight.ArrivalAirport;
            existingFlight.DepartureTime = updatedFlight.DepartureTime;
            existingFlight.BasePrice = updatedFlight.BasePrice;
            WriteFlightsToFile(_flights);
        }
    }

    public void Delete(int id)
    {
        var flightToDelete = _flights.FirstOrDefault(f => f.Id == id);
        if (flightToDelete == null)
            throw new KeyNotFoundException("Flight not found.");

        _flights.Remove(flightToDelete);
        WriteFlightsToFile(_flights);
    }

    public void SaveAll(ICollection<Flight> flights)
    {
        _flights.Clear();
        _flights.AddRange(flights);
        WriteFlightsToFile(_flights);
    }

    private static void WriteFlightsToFile(List<Flight> flights)
    {
        var lines = new List<string> { "Id,DepartureCountry,DestinationCountry,DepartureAirport,ArrivalAirport,DepartureTime,Price" };
        
        lines.AddRange(flights.Select(f => FlightsParser.ToLine(
            f.Id,
            f.DepartureCountry,
            f.DestinationCountry,
            f.DepartureAirport,
            f.ArrivalAirport,
            f.DepartureTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
            f.BasePrice.ToString(CultureInfo.InvariantCulture))));
        
        File.WriteAllLines(FlightsFilePath, lines);
    }
    public IReadOnlyCollection<Flight> SearchFlights(FlightFilter filter)
    {
        return _flights.Where(f =>
            (string.IsNullOrEmpty(filter.DepartureCountry) || f.DepartureCountry?.Equals(filter.DepartureCountry, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.DestinationCountry) || f.DestinationCountry?.Equals(filter.DestinationCountry, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.DepartureAirport) || f.DepartureAirport?.Equals(filter.DepartureAirport, StringComparison.OrdinalIgnoreCase) == true) &&
            (string.IsNullOrEmpty(filter.ArrivalAirport) || f.ArrivalAirport?.Equals(filter.ArrivalAirport, StringComparison.OrdinalIgnoreCase) == true) &&
            (!filter.DepartureDate.HasValue || f.DepartureTime.Date == filter.DepartureDate.Value.Date) &&
            (!filter.MaxPrice.HasValue || f.GetPriceForClass(filter.FlightClass ?? FlightClass.Economy) <= filter.MaxPrice.Value)
        ).ToList();
    }

    IReadOnlyCollection<Booking> IBookingRepository.GetAll()
    {
        return _bookingRepository.GetAll();
    }

    public void Add(Booking booking)
    {
        _bookingRepository.Add(booking);
    }

    public void Update(Booking updatedBooking)
    {
        _bookingRepository.Update(updatedBooking);
    }

    void IBookingRepository.Delete(int id)
    {
        _bookingRepository.Delete(id);
    }

    public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
    {
        var bookings = _bookingRepository.GetAll();

        return bookings.Where(b =>
        {
            var flight = _flights.FirstOrDefault(f => f.Id == b.FlightId);
            if (flight == null)
                return false;

            return (!filter.FlightId.HasValue || b.FlightId == filter.FlightId.Value) &&
                (!filter.MaxPrice.HasValue || b.PricePaid <= filter.MaxPrice.Value) &&
                (string.IsNullOrEmpty(filter.PassengerEmail) || b.Passenger.Email!.Equals(filter.PassengerEmail, StringComparison.OrdinalIgnoreCase)) &&
                (!filter.FlightClass.HasValue || b.SelectedClass == filter.FlightClass.Value) &&
                (string.IsNullOrEmpty(filter.DepartureCountry) || flight.DepartureCountry!.Equals(filter.DepartureCountry, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(filter.DestinationCountry) || flight.DestinationCountry!.Equals(filter.DestinationCountry, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(filter.DepartureAirport) || flight.DepartureAirport!.Equals(filter.DepartureAirport, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(filter.ArrivalAirport) || flight.ArrivalAirport!.Equals(filter.ArrivalAirport, StringComparison.OrdinalIgnoreCase)) &&
                (!filter.DepartureDate.HasValue || flight.DepartureTime.Date == filter.DepartureDate.Value.Date);
        }).ToList();
    }

    void IBookingRepository.SaveAll(ICollection<Booking> bookings)
    {
        _bookingRepository.SaveAll(bookings);
    }
}