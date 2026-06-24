using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using TASK2.Models;

namespace TASK2.File_Storage;

public class BookingRepository
{
    private const string FilePath = "bookings.csv";

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

            var columns = line.Split(',');
            if (columns.Length == 7)
            {
                var booking = new Booking
                {
                    Id = int.Parse(columns[0]),
                    FlightId = int.Parse(columns[1]),
                    PassengerEmail = columns[2],
                    PassengerName = columns[3],
                    PassengerPhone = columns[4],
                    SelectedClass = Enum.Parse<FlightClass>(columns[5]),
                    PricePaid = decimal.Parse(columns[6], CultureInfo.InvariantCulture)
                };
                bookings.Add(booking);
            }
        }
        return bookings;
    }

    public void AddBooking(Booking booking)
    {
        var bookings = GetAll();
        var maxId = bookings.Count > 0 ? bookings.Max(b => b.Id) : 0;
        booking.Id = maxId + 1;
        
        bookings.Add(booking);
        SaveAllBookings(bookings);
    }

    public void UpdateBooking(Booking updatedBooking)
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
            
            SaveAllBookings(bookings);
        }
    }

    public void DeleteBooking(int id)
    {
        var bookings = GetAll();
        var bookingToDelete = bookings.FirstOrDefault(b => b.Id == id);
        if (bookingToDelete != null)
        {
            bookings.Remove(bookingToDelete);
            SaveAllBookings(bookings);
        }
    }

    public void SaveAllBookings(List<Booking> bookings)
    {
        var lines = new List<string> { "Id,FlightId,PassengerEmail,PassengerName,PassengerPhone,SelectedClass,PricePaid" };
        lines.AddRange(bookings.Select(b => $"{b.Id},{b.FlightId},{b.PassengerEmail},{b.PassengerName},{b.PassengerPhone},{b.SelectedClass},{b.PricePaid.ToString(CultureInfo.InvariantCulture)}"));
        File.WriteAllLines(FilePath, lines);
    }
}