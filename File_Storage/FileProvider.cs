namespace TASK2.File_Storage;

public class FileProvider : IFileProvider
{
    public bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    public string[] ReadAllLines(string filePath)
    {
        return File.ReadAllLines(filePath);
    }
}
