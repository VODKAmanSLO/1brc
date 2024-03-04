using System.Collections.Concurrent;

namespace _1brc;

public class FileReadTask
{
    private readonly string _filePath;
    private readonly int _startLine;
    private readonly int _endLine;
    private readonly ConcurrentDictionary<string, float[]> _sharedDictionary;

    public FileReadTask(string filePath, int startLine, int endLine,
        ConcurrentDictionary<string, float[]> sharedDictionary)
    {
        _filePath = filePath;
        _startLine = startLine;
        _endLine = endLine;
        _sharedDictionary = sharedDictionary;
    }

    public void Run()
    {
        using var reader = new StreamReader(_filePath);
        var currentLine = 1;

        while (reader.ReadLine() is { } line)
        {
            if (currentLine >= _startLine && currentLine <= _endLine)
            {
                var (name, tempString) = SplitString(line);
                var temperature = ParseFloat(tempString);

                _sharedDictionary.TryGetValue(name, out var value);
                if (value == null)
                {
                    _sharedDictionary.TryAdd(name, [
                        temperature, temperature, temperature, 1
                    ]);

                    continue;
                }

                value[2] += temperature;
                if (value[0] > temperature)
                {
                    value[0] = temperature;
                }

                if (value[1] < temperature)
                {
                    value[1] = temperature;
                }

                value[3]++;

                // counter++;
                // if (counter % 100000000 == 0)
                // {
                //     Console.WriteLine($"Processed {counter / 1000000}M lines");
                // }
            }

            currentLine++;
        }
    }


    private static (string Name, string Temp) SplitString(string input)
    {
        var separatorIndex = input.IndexOf(';');
        var name = input[..separatorIndex];
        var temp = input[(separatorIndex + 1)..];
        return (name, temp);
    }

    private static float ParseFloat(string input)
    {
        var result = 0f;
        var sign = 1;
        var decimalPoint = false;
        var decimalMultiplier = 0.1f;

        for (var i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (c == '-')
            {
                sign = -1;
            }
            else if (c == '.')
            {
                decimalPoint = true;
            }
            else
            {
                if (decimalPoint)
                {
                    result += (c - '0') * decimalMultiplier;
                    decimalMultiplier *= 0.1f;
                }
                else
                {
                    result = result * 10 + (c - '0');
                }
            }
        }

        return result * sign;
    }
}