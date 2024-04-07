using IPParser.Interface;
using IPParser.Services;
using Moq;

namespace IPParser.Tests.LogParserTests;

[TestFixture]
public class ParseLogFileTests
{
    private Mock<IFileReader> _mockFileReader;
    private LogService _logParser;

    [SetUp]
    public void SetUp()
    {
        _mockFileReader = new Mock<IFileReader>();
        _logParser = new LogService(_mockFileReader.Object);
    }

    [Test]
    public void ParseLogFile_WithValidData_ReturnsCorrectLogEntries()
    {
        // Arrange
        var lines = new[]
        {
        "192.168.1.1:2023-03-30 14:20:10",
        "10.0.0.1:2023-04-01 10:15:00"
        };
        _mockFileReader.Setup(m => m.ReadAllLines(It.IsAny<string>())).Returns(lines);

        // Act
        var result = _logParser.ReadLogEntriesFromFile("dummyPath");

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("192.168.1.1", result[0].IpAddress);
        Assert.AreEqual(new DateTime(2023, 03, 30, 14, 20, 10), result[0].AccessTime);
        Assert.AreEqual("10.0.0.1", result[1].IpAddress);
        Assert.AreEqual(new DateTime(2023, 04, 01, 10, 15, 0), result[1].AccessTime);
    }

    [Test]
    public void ParseLogFile_WithInvalidData_IgnoresBadLines()
    {
        // Arrange
        var lines = new string[] {
            "192.168.1.1:bad-date",
            "invalid-format"
        };
        _mockFileReader.Setup(x => x.ReadAllLines(It.IsAny<string>())).Returns(lines);

        // Act
        var entries = _logParser.ReadLogEntriesFromFile("dummyPath");

        // Assert
        Assert.AreEqual(0, entries.Count);
    }

    [Test]
    public void ParseLogFile_WithEmptyFile_ReturnsEmptyList()
    {
        // Arrange
        _mockFileReader.Setup(x => x.ReadAllLines(It.IsAny<string>())).Returns(new string[] { });

        // Act
        var entries = _logParser.ReadLogEntriesFromFile("dummyPath");

        // Assert
        Assert.AreEqual(0, entries.Count);
    }
}
