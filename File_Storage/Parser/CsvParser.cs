using TASK2.Extensions;

namespace TASK2.File_Storage.Parser;
public class CsvParser : IParser
{
    /// <inheritdoc />
    public string[] ParseLine(string line)
    {
        return line.Split(',');
    }

    /// <inheritdoc />
    public string ToLine(params object?[] values)
    {
        return string.Join(",", values.Select(value =>
        {
            var text = value?.ToString() ?? string.Empty;

            if (!text.IsValidSimpleValue())
            {
                throw new ArgumentException("CSV values cannot contain commas or new lines.");
            }

            return text;
        }));
    }
}