using Newtonsoft.Json;

namespace IPParser.Models;

public record UserDataDto
{
    [JsonProperty("file-log")]
    public string FileLog { get; set; }

    [JsonProperty("file-output")]
    public string FileOutput { get; set; }

    [JsonProperty("address-start")]
    public string AddressStart { get; set; }

    [JsonProperty("address-mask")]
    public string AddressMask { get; set; }

    [JsonProperty("time-start")]
    public string TimeStart { get; set; }

    [JsonProperty("time-end")]
    public string TimeEnd { get; set; }
}
