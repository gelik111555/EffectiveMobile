using IPParser.Models;

namespace IPParser.Interface;

public interface ILogQueryConfigurator
{
    LogQueryParameters LoadParametersFromJson(string filePath);
}
