using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.Models;

namespace TASK2.File_Storage.Flights;
using TASK2.File_Storage.Bookings;
using TASK2.File_Storage.Parser;


public class FlightRepository : IFlightRepository, IBookingRepository
{
    private static readonly string FlightsFilePath = StoragePath.Resolve(AppConstants.FlightsFileName);
    private static readonly string BookingsFilePath = StoragePath.Resolve(AppConstants.BookingsFileName);
    private static readonly IParser FlightsParser = ParserFactory.GetParser(Path.GetExtension(FlightsFilePath).TrimStart('.'));
    private static readonly IParser BookingsParser = ParserFactory.GetParser(Path.GetExtension(BookingsFilePath).TrimStart('.'));
    private readonly List<Flight> _flights;
    private readonly List<Booking> _bookings;

    public FlightRepository()
    {
        _flights = LoadFlightsFromFile();
        _bookings = LoadBookingsFromFile();
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
        if (flightToDelete != null)
        {
            _flights.Remove(flightToDelete);
            WriteFlightsToFile(_flights);
        }
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
        return _bookings.ToList();
    }

    private List<Booking> LoadBookingsFromFile()
    {
        var bookings = new List<Booking>();

        if (!File.Exists(BookingsFilePath) || File.ReadLines(BookingsFilePath).Count() <= 1)
            return bookings;

        var lines = File.ReadAllLines(BookingsFilePath);
        lines = lines.Skip(1).ToArray();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = BookingsParser.ParseLine(line);

            if (columns.Length == 7 &&
                int.TryParse(columns[0], out var id) &&
                int.TryParse(columns[1], out var flightId) &&
                Enum.TryParse(columns[5], out FlightClass selectedClass) &&
                decimal.TryParse(columns[6], NumberStyles.Number, CultureInfo.InvariantCulture, out var pricePaid))
            {
                var booking = new Booking
                {
                    Id = id,
                    FlightId = flightId,
                    Passenger = new Passenger
                    {
                        Email = columns[2],
                        Name = columns[3],
                        Phone = columns[4]
                    },
                    SelectedClass = selectedClass,
                    PricePaid = pricePaid
                };
                bookings.Add(booking);
            }
        }
        return bookings;
    }

    public void Add(Booking booking)
    {
        var maxId = _bookings.Count > 0 ? _bookings.Max(b => b.Id) : 0;
        booking.Id = maxId + 1;

        _bookings.Add(booking);
        WriteBookingsToFile(_bookings);
    }

    public void Update(Booking updatedBooking)
    {
        var existingBooking = _bookings.FirstOrDefault(b => b.Id == updatedBooking.Id);
        if (existingBooking != null)
        {
            existingBooking.FlightId = updatedBooking.FlightId;
            existingBooking.Passenger = updatedBooking.Passenger;
            existingBooking.SelectedClass = updatedBooking.SelectedClass;
            existingBooking.PricePaid = updatedBooking.PricePaid;

            WriteBookingsToFile(_bookings);
        }
    }

    void IBookingRepository.Delete(int id)
    {
        var bookingToDelete = _bookings.FirstOrDefault(b => b.Id == id);
        if (bookingToDelete != null)
        {
            _bookings.Remove(bookingToDelete);
            WriteBookingsToFile(_bookings);
        }
    }

    public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
    {
        var allFlights = _flights;

        return _bookings.Where(b =>
        {
            var flight = allFlights.FirstOrDefault(f => f.Id == b.FlightId);
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
        _bookings.Clear();
        _bookings.AddRange(bookings);
        WriteBookingsToFile(_bookings);
    }

    private static void WriteBookingsToFile(ICollection<Booking> bookings)
    {
        var lines = new List<string> { "Id,FlightId,PassengerEmail,PassengerName,PassengerPhone,SelectedClass,PricePaid" };
        lines.AddRange(bookings.Select(b => BookingsParser.ToLine(
            b.Id,
            b.FlightId,
            b.Passenger.Email,
            b.Passenger.Name,
            b.Passenger.Phone,
            b.SelectedClass,
            b.PricePaid.ToString(CultureInfo.InvariantCulture))));
        File.WriteAllLines(BookingsFilePath, lines);
    }       

    
}
