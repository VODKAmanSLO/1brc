using System.Diagnostics;

Console.WriteLine("Starting!");
var startTime = Stopwatch.StartNew();

var lines = File.ReadLines(@"C:\code\1brcFork\measurements.txt");
var counter = 0;
var dict = new Dictionary<string, float[]>();

foreach (var line in lines)
{
    var (name, tempString) = SplitString(line);
    var temperature = ParseFloat(tempString);

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
    if (counter % 100000000 == 0)
    {
        Console.WriteLine($"Processed {counter / 1000000}M lines");
    }

    // if (counter == 100000000)
    // {
    //     break;
    // }
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

// Create a super optimized method for parsing strings to floats. Don't worry about readability, we need speed!
// The input will always be in the format "31.4", but may sometimes include a sign, like "-31.4".
// Don't forget about the decimal point! 
static float ParseFloat(string input)
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