using TASK2.Models;
namespace TASK2.File_Storage;
public class FlightRepository

{
    private const string FilePath = "flights.csv";
    public List<Flight> GetAll()
    {

        var lines = File.ReadAllLines(FilePath);
        var flights = new List<Flight>();
        lines = lines.Skip(1).ToArray();
        
        foreach (var line in lines)
        {
            var columns = line.Split(',');
            var flight = new Flight
            {
                Id = int.Parse(columns[0]),
                DepartureCountry = columns[1],
                DestinationCountry = columns[2],
                DepartureAirport = columns[3],
                ArrivalAirport = columns[4],
                DepartureTime = DateTime.Parse(columns[5]),
                Price = decimal.Parse(columns[6])
            };
            flights.Add(flight);
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
        existingFlight.Price = updatedFlight.Price;
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
lines.AddRange(flights.Select(f => $"{f.Id},{f.DepartureCountry},{f.DestinationCountry},{f.DepartureAirport},{f.ArrivalAirport},{f.DepartureTime:yyyy-MM-dd},{f.Price}"));        File.WriteAllLines(FilePath, lines);
        
    }

}
