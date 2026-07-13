using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TASK2.File_Storage.Parser;
using TASK2.Models;

namespace TASK2.File_Storage.Bookings;

public class BookingRepository : IBookingRepository
{
    private static readonly string BookingsFilePath = StoragePath.Resolve(AppConstants.BookingsFileName);
    private static readonly IParser BookingsParser = ParserFactory.GetParser(Path.GetExtension(BookingsFilePath).TrimStart('.'));
    private readonly List<Booking> _bookings;

    public BookingRepository()
    {
        _bookings = LoadBookingsFromFile();
    }

    public IReadOnlyCollection<Booking> GetAll()
    {
        return _bookings.ToList();
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
        if (existingBooking == null)
            return;

        existingBooking.FlightId = updatedBooking.FlightId;
        existingBooking.Passenger = updatedBooking.Passenger;
        existingBooking.SelectedClass = updatedBooking.SelectedClass;
        existingBooking.PricePaid = updatedBooking.PricePaid;

        WriteBookingsToFile(_bookings);
    }

    public void Delete(int id)
    {
        var bookingToDelete = _bookings.FirstOrDefault(b => b.Id == id);
        if (bookingToDelete == null)
            return;

        _bookings.Remove(bookingToDelete);
        WriteBookingsToFile(_bookings);
    }

    public IReadOnlyCollection<Booking> FilterBookings(BookingFilter filter)
    {
        return _bookings.Where(b =>
            (!filter.FlightId.HasValue || b.FlightId == filter.FlightId.Value) &&
            (!filter.MaxPrice.HasValue || b.PricePaid <= filter.MaxPrice.Value) &&
            (string.IsNullOrEmpty(filter.PassengerEmail) || b.Passenger.Email.Value.Equals(filter.PassengerEmail, StringComparison.OrdinalIgnoreCase)) &&
            (!filter.FlightClass.HasValue || b.SelectedClass == filter.FlightClass.Value)
        ).ToList();
    }

    public void SaveAll(ICollection<Booking> bookings)
    {
        _bookings.Clear();
        _bookings.AddRange(bookings);
        WriteBookingsToFile(_bookings);
    }

    private static List<Booking> LoadBookingsFromFile()
    {
        var bookings = new List<Booking>();

        if (!File.Exists(BookingsFilePath) || File.ReadLines(BookingsFilePath).Count() <= 1)
            return bookings;

        var lines = File.ReadAllLines(BookingsFilePath).Skip(1);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = BookingsParser.ParseLine(line);

            if (columns.Length == 7 &&
                int.TryParse(columns[0], out var id) &&
                int.TryParse(columns[1], out var flightId) &&
                Enum.TryParse(columns[5], out FlightClass selectedClass) &&
                decimal.TryParse(columns[6], NumberStyles.Number, CultureInfo.InvariantCulture, out var pricePaid))
            {
                bookings.Add(new Booking
                {
                    Id = id,
                    FlightId = flightId,
                    Passenger = new Passenger(
                        new Email(columns[2]),
                        columns[3],
                        columns[4]),
                    SelectedClass = selectedClass,
                    PricePaid = pricePaid
                });
            }
        }

        return bookings;
    }

    private static void WriteBookingsToFile(ICollection<Booking> bookings)
    {
        var lines = new List<string> { "Id,FlightId,PassengerEmail,PassengerName,PassengerPhone,SelectedClass,PricePaid" };
        lines.AddRange(bookings.Select(b => BookingsParser.ToLine(
            b.Id,
            b.FlightId,
            b.Passenger.Email.Value,
            b.Passenger.Name,
            b.Passenger.Phone,
            b.SelectedClass,
            b.PricePaid.ToString(CultureInfo.InvariantCulture))));
        File.WriteAllLines(BookingsFilePath, lines);
    }
}