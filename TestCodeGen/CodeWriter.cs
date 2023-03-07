//  Copyright (c) 2023-present John Roscoe Hamilton
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace TestCodeGen; 

/// <summary>
/// Helper class for generating source code.
/// </summary>
public class CodeWriter
{
    int indentCount = 0;
    List<string> lines = new List<string>();
    string appendText = "";
    public string line
    {
        set
        {
            Add(value);
        }
    }
    public void Add(string value)
    {
        int curIndentCount = indentCount;
        if (value == "}" || value == "};" || value == "},")
        {
            indentCount--;
            curIndentCount--;
        }
        if (value == "{")
        {
            indentCount++;
        }
        string tabText = new string('\t', curIndentCount) + "";
        lines.Add(tabText + appendText + value);
        appendText = "";
    }
    public void AddLines(string linesAsString)
    {
        string tabText = new string('\t', indentCount) + "";
        var result = GetLines(linesAsString);
        foreach (string line in result) 
        { 
            Lines.Add(tabText + line);
        }
    }
    public string[] GetLines(string linesAsString)
    {
        return Regex.Split(linesAsString, "\r\n|\r|\n");
    }
    public string append
    {
        set
        {
            appendText += value;
        }
    }
    public List<string> Lines
    {
        get
        {
            if (appendText.Length > 0)
                lines.Add(appendText);
            return lines;
        }
    }
    public SourceText Source()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var l in lines)
        {
            sb.AppendLine(l);
        }
        return SourceText.From(sb.ToString(), Encoding.UTF8);
    }
    public void Clear()
    {
        lines.Clear();
    }
}