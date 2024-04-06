using Newtonsoft.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    public static LogQueryParameters LoadParametersFromJson(string filePath)
    {
        var jsonText = File.ReadAllText(filePath);
        var parameters = JsonConvert.DeserializeObject<LogQueryParameters>(jsonText);
        return parameters;
    }
}
