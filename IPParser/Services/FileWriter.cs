using IPParser.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPParser.Services;


public class FileWriter : IFileWriter
{
    public void WriteAllText(string path, string text)
    {
        try
        {
            File.WriteAllText(path, text);
        }
        catch (IOException ex)
        {
            throw new IOException($"An error occurred while writing to the file: {ex.Message}", ex);
        }
    }

    public void WriteAllLines(string path, string[] lines)
    {
        try
        {
            File.WriteAllLines(path, lines);
        }
        catch (IOException ex)
        {
            throw new IOException($"An error occurred while writing to the file: {ex.Message}", ex);
        }
    }
}

