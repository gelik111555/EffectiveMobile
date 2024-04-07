﻿namespace IPParser.Interface;

public interface IFileReader
{
    string ReadAllText(string path);
    string[] ReadAllLines(string path);
}
