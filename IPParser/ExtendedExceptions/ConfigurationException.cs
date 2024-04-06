namespace IPParser.ExtendedExceptions;

public class ConfigurationException : Exception
{
    public string FilePath { get; }

    public ConfigurationException(string message, string filePath)
        : base($"{message} Path: {filePath}")
    {
        FilePath = filePath;
    }
}
