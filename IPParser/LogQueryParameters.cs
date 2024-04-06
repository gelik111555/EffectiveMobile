using Newtonsoft.Json;
using System.Net;

public struct LogQueryParameters
{
    [JsonProperty("file-log")]
    public string FileLog { get; }

    [JsonProperty("file-output")]
    public string FileOutput { get; }

    [JsonProperty("address-start")]
    public IPAddress? AddressStart { get; }

    [JsonProperty("address-mask")]
    public int? AddressMask { get; }

    [JsonProperty("time-start")]
    public DateOnly TimeStart { get; }

    [JsonProperty("time-end")]
    public DateOnly TimeEnd { get; }

    [JsonConstructor]
    public LogQueryParameters(
        [JsonProperty("file-log")] string fileLog,
        [JsonProperty("file-output")] string fileOutput,
        [JsonProperty("address-start")] string addressStart,
        [JsonProperty("address-mask")] string? addressMask,
        [JsonProperty("time-start")] string timeStart,
        [JsonProperty("time-end")] string timeEnd)
    {
        FileLog = fileLog ?? throw new ArgumentNullException(nameof(fileLog));
        FileOutput = fileOutput ?? throw new ArgumentNullException(nameof(fileOutput));

        AddressStart = !string.IsNullOrWhiteSpace(addressStart) ? IPAddress.Parse(addressStart) : null;

        AddressMask = !string.IsNullOrWhiteSpace(addressMask) ? int.Parse(addressMask) : (int?)null;

        var culture = System.Globalization.CultureInfo.InvariantCulture;
        TimeStart = DateOnly.ParseExact(timeStart, "dd.MM.yyyy", culture);
        TimeEnd = DateOnly.ParseExact(timeEnd, "dd.MM.yyyy", culture);
    }
}