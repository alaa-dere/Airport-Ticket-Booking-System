namespace TASK2.File_Storage;
public interface IParser
{
 public  string[] ParseLine(string line);  
 public string ToLine(params object?[] values);  
  public bool IsValidSimpleValue(string value);
}