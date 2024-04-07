using IPParser.Interface;
using System.IO;

namespace IPParser.Services;

public class FileReader : IFileReader
{
    public string ReadAllText(string path)
    {
        EnsureFileExists(path);
        return ReadFile(path, File.ReadAllText);
    }

    public string[] ReadAllLines(string path)
    {
        EnsureFileExists(path);
        return ReadFile(path, File.ReadAllLines);
    }

    private static void EnsureFileExists(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"The file at {path} was not found.");
        }
    }

    private static T ReadFile<T>(string path, Func<string, T> readFunc)
    {
        try
        {
            return readFunc(path);
        }
        catch (IOException ex)
        {
            throw new IOException($"An error occurred while reading the file: {ex.Message}", ex);
        }
    }
}