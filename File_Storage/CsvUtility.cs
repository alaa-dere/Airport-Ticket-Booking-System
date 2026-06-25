using System.Text;

namespace TASK2.File_Storage;

public static class CsvUtility
{
    public static string[] ParseLine(string line)
    {
        var columns = new List<string>();
        var currentColumn = new StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var currentChar = line[i];

            if (currentChar == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    currentColumn.Append('"');
                    i++;
                }
                else
                {
                    insideQuotes = !insideQuotes;
                }

                continue;
            }

            if (currentChar == ',' && !insideQuotes)
            {
                columns.Add(currentColumn.ToString());
                currentColumn.Clear();
                continue;
            }

            currentColumn.Append(currentChar);
        }

        columns.Add(currentColumn.ToString());
        return columns.ToArray();
    }

    public static string ToLine(params object?[] values)
    {
        return string.Join(",", values.Select(value => Escape(value?.ToString() ?? string.Empty)));
    }

    private static string Escape(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
