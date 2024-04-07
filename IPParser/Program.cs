using System;
using FluentValidation;
using IPParser.Interface;
using IPParser.Models;
using IPParser.Services;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            IFileReader _reader = new FileReader();
            var parameters = ChooseInputMethod(_reader);
            LogService logService = new(_reader);
            var logs = logService.ReadLogEntriesFromFile(parameters.FileLog);
            var results = LogService.FilterLogEntriesByParameters(logs, parameters);
            WriteLogEntriesToFile(parameters.FileOutput, results);
            Console.WriteLine("The work has been completed successfully");
        }
        catch(ValidationException ex)
        {
            foreach (var item in ex.Errors)
            { 
                Console.WriteLine($"Error: {item.AttemptedValue} - {item.ErrorMessage}");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            var type = ex.GetType().ToString(); 
            Console.WriteLine($"Exception type : {type}");
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static LogQueryParameters ChooseInputMethod(IFileReader _reader)
    {
        ILogQueryConfigurator configurator = new LogQueryConfigurator(_reader);
        while (true)
        {
            Console.WriteLine("Choose input method:");
            Console.WriteLine("1 - Load from JSON file");
            Console.WriteLine("2 - Enter data manually");
            var inputMethod = Console.ReadLine();
            switch (inputMethod)
            {
                case "1":
                    while (true)
                    {
                        Console.WriteLine("Enter the path to the configuration file:");
                        var filePath = Console.ReadLine();
                        if (string.IsNullOrEmpty(filePath) || !Path.Exists(filePath)) // Используем !Path.Exists
                        {
                            Console.WriteLine("Incorrect file path, please try again.");
                            continue;
                        }
                        return configurator.LoadParametersFromJson(filePath);
                    }

                case "2":
                    var userData = ReadUserDataFromConsole();
                    return LogQueryConfigurator.LoadParametersFromUserData(userData);

                default:
                    Console.WriteLine("Invalid input method selected, please try again.");
                    continue;
            }
        }
    }

    private static UserDataDto ReadUserDataFromConsole()
    {
        Console.WriteLine("Enter File Log:");
        var fileLog = Console.ReadLine();

        Console.WriteLine("Enter File Output:");
        var fileOutput = Console.ReadLine();

        Console.WriteLine("Enter Address Start (optional):");
        var addressStart = Console.ReadLine();

        Console.WriteLine("Enter Address Mask (optional):");
        var addressMask = Console.ReadLine();

        Console.WriteLine("Enter Time Start (dd.MM.yyyy):");
        var timeStart = Console.ReadLine();

        Console.WriteLine("Enter Time End (dd.MM.yyyy):");
        var timeEnd = Console.ReadLine();

        return new UserDataDto
        {
            FileLog = fileLog,
            FileOutput = fileOutput,
            AddressStart = addressStart,
            AddressMask = addressMask,
            TimeStart = timeStart,
            TimeEnd = timeEnd
        };
    }

    private static void WriteLogEntriesToFile(string filePath, List<LogEntry> logEntries)
    {
        IFileWriter fileWriter = new FileWriter();
        var lines = logEntries.Select
            (entry => $"{entry.IpAddress}:{entry.AccessTime.ToString("yyyy-MM-dd HH:mm:ss")}").ToArray();
        fileWriter.WriteAllLines(filePath, lines);
    }
}
