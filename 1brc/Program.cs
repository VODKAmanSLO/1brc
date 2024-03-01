using System.Diagnostics;
using System.Globalization;

Console.WriteLine("Starting!");
var startTime = Stopwatch.StartNew();

var lines = File.ReadLines("../../../../data/measurements.txt");

var dict = new Dictionary<string, float[]>();

foreach (var line in lines)
{
    var (name, tempString) = SplitString(line);
    var temperature = float.Parse(tempString, CultureInfo.InvariantCulture);

    dict.TryGetValue(name, out var value);
    if (value == null)
    {
        dict.Add(name, [
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
}

var orderedDict = dict.OrderBy(d => d.Key);
foreach (var (key, temperature) in orderedDict)
{
    var avg = temperature[2] / temperature[3];
    Console.WriteLine($"{key}={temperature[0]}/{avg:n2}/{temperature[1]}");
}

Console.WriteLine($"End time: {startTime.Elapsed.Seconds}.{startTime.Elapsed.Milliseconds}");
return;

static (string Name, string Temp) SplitString(string input)
{
    var separatorIndex = input.IndexOf(';');
    var name = input[..separatorIndex];
    var temp = input[(separatorIndex + 1)..];
    return (name, temp);
}