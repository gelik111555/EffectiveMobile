using IPParser.ExtendedExceptions;
using IPParser.Interface;
using Newtonsoft.Json;

namespace IPParser;

public class LogQueryConfigurator
{
    private readonly IFileReader _fileReader;

    public LogQueryConfigurator(IFileReader fileReader)
    {
        _fileReader = fileReader;
    }
    public LogQueryParameters LoadParametersFromJson(string filePath)
    {
        try
        {
            var jsonText = _fileReader.ReadAllText(filePath);
            var parameters = JsonConvert.DeserializeObject<LogQueryParameters>(jsonText);
            return parameters;
        }
        catch (FileNotFoundException)
        {
            throw new ConfigurationException("Configuration file not found.", filePath);
        }
        catch (JsonSerializationException)
        {
            throw new ConfigurationException("Invalid configuration format.", filePath);
        }
    }

}
