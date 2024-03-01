using System.Diagnostics;
using System.Globalization;

Console.WriteLine("Starting!");
var startTime = Stopwatch.StartNew();

var lines = File.ReadLines(@"C:\code\1brcFork\measurements.txt");
var counter = 0;
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

    counter++;
    if (counter % 10000000 == 0)
    {
        Console.WriteLine($"Processed {counter / 1000000}M lines");
    }
}

Console.WriteLine($"After reading: {startTime.Elapsed.Seconds}.{startTime.Elapsed.Milliseconds}");

var orderedDict = dict.OrderBy(d => d.Key);
foreach (var (key, value) in orderedDict)
{
    var avg = value[2] / value[3];
    Console.WriteLine($"{key}={value[0]}/{avg:n1}/{value[1]}");
}

Console.WriteLine(
    $"End time: {startTime.Elapsed.Minutes}:{startTime.Elapsed.Seconds}.{startTime.Elapsed.Milliseconds}");
Console.WriteLine($"Counters: {counter}");
return;

static (string Name, string Temp) SplitString(string input)
{
    var separatorIndex = input.IndexOf(';');
    var name = input[..separatorIndex];
    var temp = input[(separatorIndex + 1)..];
    return (name, temp);
}