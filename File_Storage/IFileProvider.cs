namespace TASK2.File_Storage;

public interface IFileProvider
{
    bool Exists(string filePath);

    string[] ReadAllLines(string filePath);
}
