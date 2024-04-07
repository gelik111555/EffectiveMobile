using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPParser.Interface;

internal interface IFileWriter
{
    void WriteAllText(string path, string text);
    void WriteAllLines(string path, string[] lines);
}
