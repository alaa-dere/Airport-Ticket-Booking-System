namespace TASK2.File_Storage.Parser
{
    public interface IParser
    {
        public  string[] ParseLine(string line);  
        public string ToLine(params object?[] values);  
        public bool IsValidSimpleValue(string value);
    }
}