namespace TASK2.File_Storage;

public static class CsvUtility
{
    public static string[] ParseLine(string line)
    {
        return line.Split(',');
    }

    public static string ToLine(params object?[] values)
    {
        return string.Join(",", values.Select(value =>
        {
            var text = value?.ToString() ?? string.Empty;

            if (!IsValidSimpleValue(text))
            {
                throw new ArgumentException("CSV values cannot contain commas or new lines.");
            }

            return text;
        }));
    }

    public static bool IsValidSimpleValue(string value)
    {
        return !value.Contains(',') &&
            !value.Contains('\n') &&
            !value.Contains('\r');
    }
}
