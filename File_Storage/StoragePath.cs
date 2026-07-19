namespace TASK2.File_Storage;

public static class StoragePath
{
    public static string Resolve(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, fileName);
    }
}