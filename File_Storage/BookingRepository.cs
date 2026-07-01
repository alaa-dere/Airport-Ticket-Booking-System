using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.Models;

namespace TASK2.File_Storage;

public class BookingRepository
{
    private static readonly string FilePath = StoragePath.Resolve(AppConstants.BookingsFileName);
    private static readonly IParser Parser = ParserFactory.GetParser(Path.GetExtension(FilePath).TrimStart('.'));
    public List<Booking> GetAll()
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
                    PassengerEmail = columns[2],
                    PassengerName = columns[3],
                    PassengerPhone = columns[4],
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
        var bookings = GetAll();
        var maxId = bookings.Count > 0 ? bookings.Max(b => b.Id) : 0;
        booking.Id = maxId + 1;
        
        bookings.Add(booking);
        SaveAll(bookings);
    }

    public void Update(Booking updatedBooking)
    {
        var bookings = GetAll();
        var existingBooking = bookings.FirstOrDefault(b => b.Id == updatedBooking.Id);
        if (existingBooking != null)
        {
            existingBooking.FlightId = updatedBooking.FlightId;
            existingBooking.PassengerEmail = updatedBooking.PassengerEmail;
            existingBooking.PassengerName = updatedBooking.PassengerName;
            existingBooking.PassengerPhone = updatedBooking.PassengerPhone;
            existingBooking.SelectedClass = updatedBooking.SelectedClass;
            existingBooking.PricePaid = updatedBooking.PricePaid; 
            
            SaveAll(bookings);
        }
    }

    public void Delete(int id)
    {
        var bookings = GetAll();
        var bookingToDelete = bookings.FirstOrDefault(b => b.Id == id);
        if (bookingToDelete != null)
        {
            bookings.Remove(bookingToDelete);
            SaveAll(bookings);
        }
    }

    public void SaveAll(List<Booking> bookings)
    {
        var lines = new List<string> { "Id,FlightId,PassengerEmail,PassengerName,PassengerPhone,SelectedClass,PricePaid" };
        lines.AddRange(bookings.Select(b => Parser.ToLine(
            b.Id,
            b.FlightId,
            b.PassengerEmail,
            b.PassengerName,
            b.PassengerPhone,
            b.SelectedClass,
            b.PricePaid.ToString(CultureInfo.InvariantCulture))));
        File.WriteAllLines(FilePath, lines);
    }
}
