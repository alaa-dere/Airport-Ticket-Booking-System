using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.Models;

namespace TASK2.File_Storage.Bookings;
using TASK2.File_Storage.Flights;
using TASK2.File_Storage.Parser;


public class BookingRepository : IBookingRepository
{
    private readonly IFlightRepository _flightRepository;
    private static readonly string FilePath = StoragePath.Resolve(AppConstants.BookingsFileName);
    private static readonly IParser Parser = ParserFactory.GetParser(Path.GetExtension(FilePath).TrimStart('.'));
    private readonly List<Booking> _bookings;

    public BookingRepository(IFlightRepository flightRepository)
{
    _flightRepository = flightRepository;
    _bookings = LoadBookingsFromFile();
}
    public IReadOnlyCollection<Booking> GetAll()
    {
        return _bookings.ToList();
    }

    private List<Booking> LoadBookingsFromFile()
    {
        var bookings = new List<Booking>();

        if (!File.Exists(FilePath) || File.ReadLines(FilePath).Count() <= 1)
            return bookings;

        var lines = File.ReadAllLines(FilePath);
        lines = lines.Skip(1).ToArray();
        
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var columns = Parser.ParseLine(line);

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

    public void Delete(int id)
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
            var allFlights = _flightRepository.GetAll();

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

    public void SaveAll(ICollection<Booking> bookings)
    {
        _bookings.Clear();
        _bookings.AddRange(bookings);
        WriteBookingsToFile(_bookings);
    }

    private static void WriteBookingsToFile(ICollection<Booking> bookings)
    {
        var lines = new List<string> { "Id,FlightId,PassengerEmail,PassengerName,PassengerPhone,SelectedClass,PricePaid" };
        lines.AddRange(bookings.Select(b => Parser.ToLine(
            b.Id,
            b.FlightId,
            b.Passenger.Email,
            b.Passenger.Name,
            b.Passenger.Phone,
            b.SelectedClass,
            b.PricePaid.ToString(CultureInfo.InvariantCulture))));
        File.WriteAllLines(FilePath, lines);
    }
}
