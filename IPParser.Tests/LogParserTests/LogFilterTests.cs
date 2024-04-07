using IPParser.Models;
using IPParser.Services;

namespace IPParser.Tests.LogParserTests;

[TestFixture]
public class LogFilterTests
{
    private List<LogEntry> _entries;
    private LogQueryParameters _params;

    [SetUp]
    public void Setup()
    {
        _entries = new List<LogEntry>
        {
            new LogEntry { IpAddress = "192.168.1.1", AccessTime = new DateTime(2023, 4, 1, 10, 20, 0) },
            new LogEntry { IpAddress = "192.168.1.2", AccessTime = new DateTime(2023, 4, 1, 10, 25, 0) },
            new LogEntry { IpAddress = "10.0.0.1", AccessTime = new DateTime(2023, 4, 2, 9, 0, 0) }
        };

        var userData = new UserDataDto
        {
            FileLog = "C:\\log.txt",
            FileOutput = "C:\\output.txt",
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            TimeStart = "04.03.2023",
            TimeEnd = "04.04.2023"
        };

        _params = new LogQueryParameters(userData);
    }

    [Test]
    public void FilterLogEntries_WithValidParameters_ReturnsFilteredEntries()
    {
        // Act
        var result = LogService.FilterLogEntriesByParameters(_entries, _params);

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.That(result, Has.Exactly(1).Matches<LogEntry>(le => le.IpAddress == "192.168.1.1"));
    }

    [Test]
    public void FilterLogEntries_WithNoMatchingDate_ReturnsEmpty()
    {
        // Arrange
        _params = new LogQueryParameters(new UserDataDto
        {
            FileLog = "C:\\log.txt",
            FileOutput = "C:\\output.txt",
            AddressStart = "192.168.1.0",
            AddressMask = "24",
            TimeStart = "03.04.2023",
            TimeEnd = "04.04.2023"
        });

        // Act
        var result = LogService.FilterLogEntriesByParameters(_entries, _params);

        // Assert
        Assert.IsEmpty(result);
    }
}
