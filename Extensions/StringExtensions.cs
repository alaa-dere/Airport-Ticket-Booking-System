namespace TASK2.Extensions;

public static class StringExtensions
{
    public static bool IsValidSimpleValue(this string value)
    {
        return !value.Contains(',') &&
               !value.Contains('\n') &&
               !value.Contains('\r');
    }
}