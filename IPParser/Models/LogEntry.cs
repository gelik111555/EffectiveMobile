namespace IPParser.Models;

public record LogEntry
{
    public string IpAddress { get; set; }
    public DateTime AccessTime { get; set; }
}
