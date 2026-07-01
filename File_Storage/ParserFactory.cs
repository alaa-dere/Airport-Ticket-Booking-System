namespace TASK2.File_Storage;

public static class ParserFactory
{
    public const string CsvParserType = "csv";
    public static IParser GetParser(string parserType)
    {
        switch (parserType.ToLower())
        {
            case CsvParserType:
                return new CsvParser();

            default:
                throw new NotSupportedException("Parser type is not supported.");
        }
    }
}