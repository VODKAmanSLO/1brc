// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Globalization;
using _1brc;

Console.WriteLine("Starting!");
var startTime = Stopwatch.StartNew();

var lines = File.ReadLines("../../../../data/measurements.txt");

var dict = new Dictionary<string, TempInfo>();

foreach (var line in lines)
{
    var (name, tempString) = SplitString(line);
    var temperature = float.Parse(tempString, CultureInfo.InvariantCulture);

    dict.TryGetValue(name, out var value);
    if (value == null)
    {
        dict.Add(name, new TempInfo
        {
            MinTemp = temperature,
            MaxTemp = temperature,
            Sum = temperature,
            Counter = 1
        });

        continue;
    }

    value.Sum += temperature;
    if (value.MinTemp > temperature) value.MinTemp = temperature;
    if (value.MaxTemp < temperature) value.MaxTemp = temperature;

    value.Counter++;
}

var orderedDict = dict.OrderBy(d => d.Key);
foreach (var (key, temperature) in orderedDict)
{
    var avg = temperature.Sum / temperature.Counter;
    Console.WriteLine($"{key}={temperature.MinTemp}/{avg:n2}/{temperature.MaxTemp}");
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