using System.Diagnostics;
using Beskar.Memory.Serialization;
using Beskar.Memory.Benchmarks.Live;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("==================================================");
Console.WriteLine("    Beskar Memory Serializer - Live Throughput    ");
Console.WriteLine("==================================================");
Console.ResetColor();

Console.WriteLine("Select object complexity for benchmark:");
Console.WriteLine("1. Simple Object (Ints + String)");
Console.WriteLine("2. Medium Object (Ints + String + List of Ints)");
Console.WriteLine("3. Very Complex Object (Deeply nested with multiple lists and custom types)");
Console.Write("Enter choice (1-3, default 1): ");

var choiceStr = Console.ReadLine();
var choice = choiceStr == "2" ? 2 : choiceStr == "3" ? 3 : 1;

object targetObj;
byte[] serializedBytes;

if (choice == 2)
{
    var obj = new MediumClass
    {
        Id = 42,
        Name = "Medium Live Tester",
        Numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
    };
    targetObj = obj;
    serializedBytes = BeSerializer.Serialize(obj);
    Console.WriteLine("\nSelected: Medium Object");
}
else if (choice == 3)
{
    var obj = new VeryComplexClass
    {
        Id = 999,
        Name = "Very Complex Live Tester",
        Tags = new List<string> { "high-performance", "serializer", "beskar", "live", "throughput", "multi-threaded" },
        Children = new List<SimpleClass>
        {
            new() { Id = 101, Name = "Nested child alpha", Id2 = 1001 },
            new() { Id = 102, Name = "Nested child beta", Id2 = 1002 },
            new() { Id = 103, Name = "Nested child gamma", Id2 = 1003 }
        },
        SimpleChildren = new List<MediumClass>
        {
            new() { Id = 501, Name = "Nested medium child X", Numbers = new List<int> { 10, 20, 30 } },
            new() { Id = 502, Name = "Nested medium child Y", Numbers = new List<int> { 40, 50, 60 } }
        }
    };
    targetObj = obj;
    serializedBytes = BeSerializer.Serialize(obj);
    Console.WriteLine("\nSelected: Very Complex Object");
}
else
{
    var obj = new SimpleClass
    {
        Id = 42,
        Name = "Simple Live Tester",
        Id2 = 999
    };
    targetObj = obj;
    serializedBytes = BeSerializer.Serialize(obj);
    Console.WriteLine("\nSelected: Simple Object");
}

Console.WriteLine($"Serialized Size: {serializedBytes.Length} bytes");

var workerCount = Environment.ProcessorCount;
Console.WriteLine($"Detecting {workerCount} logical cores. Launching workers...");

var serializeCounts = new long[workerCount];
var deserializeCounts = new long[workerCount];

var cts = new CancellationTokenSource();
var workers = new Task[workerCount];

for (var i = 0; i < workerCount; i++)
{
    var workerId = i;
    workers[i] = Task.Run(() => WorkerLoop(workerId, targetObj, choice, cts.Token));
}

var reporter = Task.Run(() => ReportLoop(cts.Token));

Console.WriteLine("Running. Press Enter to stop...");
Console.ReadLine();

Console.WriteLine("Stopping workers...");
cts.Cancel();
try
{
    Task.WaitAll(workers);
    reporter.Wait();
}
catch (AggregateException)
{
    // Ignore expected cancellation exceptions
}
Console.WriteLine("Stopped.");

void WorkerLoop(
    int workerId,
    object obj,
    int complexity,
    CancellationToken token)
{
    var buffer = new byte[8192];
    const int batchSize = 1000;

    if (complexity == 2)
    {
        var mediumObj = (MediumClass)obj;
        while (!token.IsCancellationRequested)
        {
            for (var i = 0; i < batchSize; i++)
            {
                BeSerializer.Serialize(mediumObj, buffer);
            }
            serializeCounts[workerId] += batchSize;

            for (var i = 0; i < batchSize; i++)
            {
                _ = BeSerializer.Deserialize<MediumClass>(serializedBytes);
            }
            deserializeCounts[workerId] += batchSize;
        }
    }
    else if (complexity == 3)
    {
        var complexObj = (VeryComplexClass)obj;
        while (!token.IsCancellationRequested)
        {
            for (var i = 0; i < batchSize; i++)
            {
                BeSerializer.Serialize(complexObj, buffer);
            }
            serializeCounts[workerId] += batchSize;

            for (var i = 0; i < batchSize; i++)
            {
                _ = BeSerializer.Deserialize<VeryComplexClass>(serializedBytes);
            }
            deserializeCounts[workerId] += batchSize;
        }
    }
    else
    {
        var simpleObj = (SimpleClass)obj;
        while (!token.IsCancellationRequested)
        {
            for (var i = 0; i < batchSize; i++)
            {
                BeSerializer.Serialize(simpleObj, buffer);
            }
            serializeCounts[workerId] += batchSize;

            for (var i = 0; i < batchSize; i++)
            {
                _ = BeSerializer.Deserialize<SimpleClass>(serializedBytes);
            }
            deserializeCounts[workerId] += batchSize;
        }
    }
}

async Task ReportLoop(CancellationToken token)
{
    var stopwatch = Stopwatch.StartNew();
    long lastSerialize = 0;
    long lastDeserialize = 0;

    while (!token.IsCancellationRequested)
    {
        try
        {
            await Task.Delay(1000, token);
        }
        catch (TaskCanceledException)
        {
            break;
        }

        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
        stopwatch.Restart();

        long currentSerialize = 0;
        long currentDeserialize = 0;

        for (var i = 0; i < serializeCounts.Length; i++)
        {
            currentSerialize += Volatile.Read(ref serializeCounts[i]);
            currentDeserialize += Volatile.Read(ref deserializeCounts[i]);
        }

        var diffSerialize = currentSerialize - lastSerialize;
        var diffDeserialize = currentDeserialize - lastDeserialize;

        lastSerialize = currentSerialize;
        lastDeserialize = currentDeserialize;

        var serializeOpsPerSec = diffSerialize / elapsedSeconds;
        var deserializeOpsPerSec = diffDeserialize / elapsedSeconds;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
        Console.ResetColor();
        Console.WriteLine(
            $"Serialize: {serializeOpsPerSec:N0} ops/s | " +
            $"Deserialize: {deserializeOpsPerSec:N0} ops/s | " +
            $"Combined: {(serializeOpsPerSec + deserializeOpsPerSec):N0} ops/s");
    }
}
