using IPParser.Interface;
using IPParser.Models;
using System.Globalization;
using System.Net;

namespace IPParser.Services;

public class LogService
{
    private readonly IFileReader _fileReader;

    public LogService(IFileReader fileReader)
    {
        _fileReader = fileReader;
    }
    public List<LogEntry> ReadLogEntriesFromFile(string filePath)
    {
        var logEntries = new List<LogEntry>();
        var lines = _fileReader.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            try
            {
                var parts = line.Split(new[] { ':' }, 2);
                if (parts.Length == 2 && IPAddress.TryParse(parts[0], out var ipAddress))
                {
                    var dateTime = DateTime.ParseExact(parts[1].Trim(), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    logEntries.Add(new LogEntry
                    {
                        IpAddress = ipAddress.ToString(),
                        AccessTime = dateTime
                    });
                }
            }
            catch
            {
                Console.WriteLine($"Invalid line format or IP address in line: {line}");
                continue;
            }
        }
        return logEntries;
    }
    public static List<LogEntry> FilterLogEntriesByParameters
        (List<LogEntry> entries, LogQueryParameters logQueryParameters)
    {
        return entries.Where(entry =>
            (logQueryParameters.AddressStart == null ||
             CheckIpMask(entry.IpAddress, logQueryParameters.AddressStart, logQueryParameters.AddressMask)) &&
            DateOnly.FromDateTime(entry.AccessTime) >= logQueryParameters.TimeStart &&
            DateOnly.FromDateTime(entry.AccessTime) <= logQueryParameters.TimeEnd
        ).ToList();
    }
    private static bool CheckIpMask(string ipAddress, IPAddress? addressStart, int? addressMask)
    {
        if (addressStart == null || addressMask == null)
        {
            return true; // Если адрес начала или маска не указаны, пропускаем проверку подсети.
        }

        // Преобразование строкового IP в объект IPAddress
        if (!IPAddress.TryParse(ipAddress, out var ip))
        {
            return false; // Некорректный IP-адрес
        }

        byte[] ipBytes = ip.GetAddressBytes();
        byte[] startBytes = addressStart.GetAddressBytes();
        byte[] maskBytes = new byte[ipBytes.Length];

        // Создание маски сети из целочисленного представления маски
        int bits = addressMask.Value;
        for (int i = 0; i < maskBytes.Length; i++)
        {
            if (bits >= 8)
            {
                maskBytes[i] = 255;
                bits -= 8;
            }
            else
            {
                maskBytes[i] = (byte)(255 << (8 - bits));
                break;
            }
        }

        // Проверка, входит ли IP в подсеть
        for (int i = 0; i < ipBytes.Length; i++)
        {
            if ((ipBytes[i] & maskBytes[i]) != (startBytes[i] & maskBytes[i]))
            {
                return false;
            }
        }

        return true;
    }


}
