using FluentValidation;
using IPParser.ExtendedExceptions;
using IPParser.Interface;
using IPParser.Models;
using IPParser.Services;
using IPParser.Validators;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;

namespace IPParser.Tests;

[TestFixture]
internal class LogQueryConfiguratorTests
{
    private Mock<IFileReader> _mockFileReader;
    private LogQueryConfigurator _configurator;

    [SetUp]
    public void SetUp()
    {
        _mockFileReader = new Mock<IFileReader>();
        _configurator = new LogQueryConfigurator(_mockFileReader.Object);

        var testData = new TestData
        {
            FileLog = "C:\\Windows\\testLog.log",
            FileOutput = "C:\\Windows\\output.log",
            AddressStart = "192.168.1.1",
            AddressMask = 24,
            TimeStart = "01.01.2020",
            TimeEnd = "31.12.2020"
        };
        var jsonTestData = JsonConvert.SerializeObject(testData);

        _mockFileReader.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns(jsonTestData);
    }

    [Test]
    public void LoadParametersFromJson_CorrectlyDeserializes()
    {
        string filePath = "dummy.json";

        var parameters = _configurator.LoadParametersFromJson(filePath);

        Assert.AreEqual("C:\\Windows\\testLog.log", parameters.FileLog);
        Assert.AreEqual("C:\\Windows\\output.log", parameters.FileOutput);
        Assert.AreEqual("192.168.1.1", parameters.AddressStart.ToString());
        Assert.AreEqual(24, parameters.AddressMask);
        Assert.AreEqual("01.01.2020", parameters.TimeStart.ToString());
        Assert.AreEqual("31.12.2020", parameters.TimeEnd.ToString());
    }

    [Test]
    public void LoadParametersFromJson_ThrowsOnMissingFile()
    {
        _mockFileReader.Setup(x => x.ReadAllText(It.IsAny<string>())).Throws<FileNotFoundException>();

        var ex = Assert.Throws<ConfigurationException>(() => _configurator.LoadParametersFromJson("missing.json"));

        Assert.That(ex.Message, Does.Contain("Configuration file not found."));
        Assert.That(ex.FilePath, Is.EqualTo("missing.json"));
    }

    [Test]
    public void LoadParametersFromJson_ThrowsOnInvalidJson()
    {
        _mockFileReader.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("invalid json");

        var ex = Assert.Throws<JsonReaderException>(() => _configurator.LoadParametersFromJson("invalid.json"));

        Assert.IsNotNull(ex);
    }

    [Test]
    public void LoadParametersFromUserData_WithValidData_ReturnsCorrectParameters()
    {
        // Arrange
        var userData = new UserDataDto
        {
            FileLog = "C:\\Windows\\testLog.log",
            FileOutput = "C:\\Windows\\output.log",
            AddressStart = "192.168.1.1",
            AddressMask = "24",
            TimeStart = "01.01.2020",
            TimeEnd = "31.12.2020"
        };

        // Act
        var result = LogQueryConfigurator.LoadParametersFromUserData(userData);

        // Assert
        Assert.AreEqual(userData.FileLog, result.FileLog);
        Assert.AreEqual(userData.FileOutput, result.FileOutput);
        Assert.AreEqual(IPAddress.Parse(userData.AddressStart), result.AddressStart);
        Assert.AreEqual(int.Parse(userData.AddressMask), result.AddressMask);
        Assert.AreEqual(DateOnly.ParseExact(userData.TimeStart, "dd.MM.yyyy", CultureInfo.InvariantCulture), result.TimeStart);
        Assert.AreEqual(DateOnly.ParseExact(userData.TimeEnd, "dd.MM.yyyy", CultureInfo.InvariantCulture), result.TimeEnd);
    }
    [Test]
    public void LoadParametersFromUserData_WithInvalidData_ThrowsArgumentException()
    {
        // Arrange
        var userData = new UserDataDto
        {
            FileLog = "", // Некорректное значение
            FileOutput = "C:\\Windows\\output.log",
            AddressStart = "notAnIPAddress",
            AddressMask = "notANumber",
            TimeStart = "invalidDate",
            TimeEnd = "31.12.2020"
        };
        var validator = new UserDataDtoValidator();

        // Act & Assert
        var validationResult = validator.Validate(userData);
        Assert.IsFalse(validationResult.IsValid); // Убедимся, что данные не прошли валидацию

        var ex = Assert.Throws<ValidationException>(() => LogQueryConfigurator.LoadParametersFromUserData(userData));
        Assert.That(ex.Message, Does.StartWith("Validation failed")); // Проверяем, что сообщение об ошибке соответствует ожидаемому
    }


}
public class TestData
{
    [JsonProperty("file-log")]
    public string FileLog { get; set; }

    [JsonProperty("file-output")]
    public string FileOutput { get; set; }

    [JsonProperty("address-start")]
    public string AddressStart { get; set; }

    [JsonProperty("address-mask")]
    public int AddressMask { get; set; }

    [JsonProperty("time-start")]
    public string TimeStart { get; set; }

    [JsonProperty("time-end")]
    public string TimeEnd { get; set; }
}