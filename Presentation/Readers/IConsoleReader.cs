using TASK2.Models;
namespace TASK2.Presentation.Readers
{
    public interface IConsoleReader
    {
        /// <summary>
        /// Reads optional text from the console.
        /// </summary>
        /// <returns>The trimmed text, or null when the input is empty.</returns>
        public string? ReadOptionalText();

        /// <summary>
        /// Reads an optional integer from the console.
        /// </summary>
        /// <returns>The integer value, or null when the input is invalid or empty.</returns>
        public int? ReadOptionalInt();

        /// <summary>
        /// Reads an optional decimal value from the console.
        /// </summary>
        /// <returns>The decimal value, or null when the input is invalid or empty.</returns>
        public decimal? ReadOptionalDecimal();

        /// <summary>
        /// Reads an optional date from the console.
        /// </summary>
        /// <returns>The date value, or null when the input is invalid or empty.</returns>
        public DateTime? ReadOptionalDate();

        /// <summary>
        /// Reads an optional flight class selection from the console.
        /// </summary>
        /// <returns>The selected flight class, or null when the input is invalid or empty.</returns>
        public FlightClass? ReadOptionalFlightClass();
    }
}