using System.Collections.Concurrent;
using System.Diagnostics;
using _1brc;

Console.WriteLine("Starting!");
var startTime = Stopwatch.StartNew();

const string filePath = @"C:\code\1brcFork\measurements.txt";
const int totalLines = 1000000000;
const int threadCount = 4;
const int linesPerThread = totalLines / threadCount;

var sharedDictionary = new ConcurrentDictionary<string, float[]>();
var threads = new List<Thread>();

for (var i = 0; i < threadCount; i++)
{
    var task = new FileReadTask(filePath, i * linesPerThread + 1, (i + 1) * linesPerThread, sharedDictionary);
    var thread = new Thread(task.Run);
    threads.Add(thread);
    thread.Start();
}

foreach (var thread in threads)
{
    thread.Join();
}

var orderedDict = sharedDictionary.OrderBy(d => d.Key);
foreach (var (key, value) in orderedDict)
{
    var avg = value[2] / value[3];
    Console.WriteLine($"{key}={value[0]}/{avg:n1}/{value[1]}");
}

Console.WriteLine(
    $"End time: {startTime.Elapsed.Minutes}:{startTime.Elapsed.Seconds}.{startTime.Elapsed.Milliseconds}");