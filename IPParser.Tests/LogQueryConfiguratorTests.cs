using IPParser.ExtendedExceptions;
using IPParser.Interface;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;

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
            FileLog = "testLog.log",
            FileOutput = "output.log",
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

        Assert.AreEqual("testLog.log", parameters.FileLog);
        Assert.AreEqual("output.log", parameters.FileOutput);
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