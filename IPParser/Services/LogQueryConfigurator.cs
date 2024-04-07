using IPParser.ExtendedExceptions;
using IPParser.Interface;
using IPParser.Models;
using Newtonsoft.Json;

namespace IPParser.Services;

public class LogQueryConfigurator: ILogQueryConfigurator
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
            var parameters = JsonConvert.DeserializeObject<UserDataDto>(jsonText);
            return new LogQueryParameters(parameters);
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
    public static LogQueryParameters LoadParametersFromUserData(UserDataDto userData)
    {
        return new LogQueryParameters(userData);
    }
}
