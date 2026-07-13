namespace TASK2.File_Storage.Parser
{
    public interface IParser
    {
        /// <summary>
        /// Parses a storage line into individual field values.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        /// <returns>The parsed field values.</returns>
        public string[] ParseLine(string line);  

        /// <summary>
        /// Converts field values into a storage line.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <returns>A formatted storage line.</returns>
        public string ToLine(params object?[] values);  
    }
}