
using TASK2.Models;
namespace TASK2.Presentation.Readers;
public class ConsoleReader : IConsoleReader
{
    public string? ReadOptionalText()
        {
            string? input = Console.ReadLine();
            return string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        }

    public int? ReadOptionalInt()
        {
            string? input = Console.ReadLine();
            return int.TryParse(input, out int value) ? value : null;
        }

    public decimal? ReadOptionalDecimal()
        {
            string? input = Console.ReadLine();
            return decimal.TryParse(input, out decimal value) ? value : null;
        }

    public DateTime? ReadOptionalDate()
        {
            string? input = Console.ReadLine();
            return DateTime.TryParse(input, out DateTime date) ? date.Date : null;
        }

    public FlightClass? ReadOptionalFlightClass()
        {
            string? input = Console.ReadLine();

            return input switch
            {
                "1" => FlightClass.Economy,
                "2" => FlightClass.Business,
                "3" => FlightClass.FirstClass,
                _ => null
            };
        }
}