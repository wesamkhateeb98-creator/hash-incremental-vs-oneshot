namespace HashBenchmark;

public static class Report
{
    private const string Header =
        "  Method       |  Time (ms) |  Allocated (B) |  Batches/s |  Messages/s |   MB/s | Gen0 | Gen1 | Gen2 | GC pause (ms)";

    public static void Loop(int loop, int loops, Measurement oneShot, Measurement incremental, Options options)
    {
        Console.WriteLine($"Loop {loop}/{loops}");

        Console.WriteLine(Header);

        Console.WriteLine(Row(oneShot, options));

        Console.WriteLine(Row(incremental, options));

        var same = oneShot.Digest.AsSpan().SequenceEqual(incremental.Digest);

        Console.WriteLine($"  Digest match: {(same ? "yes" : "NO")}  {Convert.ToHexStringLower(oneShot.Digest)}");

        Console.WriteLine();
    }

    public static void Summary(List<Measurement> oneShots, List<Measurement> incrementals, Options options)
    {
        Console.WriteLine($"Summary (average of {oneShots.Count} loops)");

        Console.WriteLine(Header);

        var oneShotAverage = Average(oneShots);

        var incrementalAverage = Average(incrementals);

        Console.WriteLine(Row(oneShotAverage, options));

        Console.WriteLine(Row(incrementalAverage, options));

        Console.WriteLine();

        Console.WriteLine($"  Time  min/max  One-shot   : {MinMs(oneShots):F3} / {MaxMs(oneShots):F3} ms");

        Console.WriteLine($"  Time  min/max  Incremental: {MinMs(incrementals):F3} / {MaxMs(incrementals):F3} ms");

        var speedRatio = oneShotAverage.Elapsed.TotalMilliseconds / incrementalAverage.Elapsed.TotalMilliseconds;

        var memoryRatio = (double)oneShotAverage.AllocatedBytes / incrementalAverage.AllocatedBytes;

        Console.WriteLine($"  Incremental is {speedRatio:F2}x the speed of one-shot and allocates {memoryRatio:F0}x less memory.");
    }

    private static string Row(Measurement m, Options options)
    {
        var totalBytes = (double)options.MessagesCount * options.MessageSize;

        var messagesPerSecond = m.BatchesPerSecond * options.MessagesCount;

        var megabytesPerSecond = m.BatchesPerSecond * totalBytes / (1024 * 1024);

        return $"  {m.Method,-12} | {m.Elapsed.TotalMilliseconds,10:F3} | {m.AllocatedBytes,14:N0} | {m.BatchesPerSecond,10:N0} | {messagesPerSecond,11:N0} | {megabytesPerSecond,6:N0} | {m.Gen0,4} | {m.Gen1,4} | {m.Gen2,4} | {m.GcPause.TotalMilliseconds,13:F3}";
    }

    // Average keeps the totals of GC counts (sum), because "3 collections in 10 loops" reads better than "0.3".
    private static Measurement Average(List<Measurement> runs) => new(
        runs[0].Method,
        TimeSpan.FromTicks((long)runs.Average(r => r.Elapsed.Ticks)),
        (long)runs.Average(r => r.AllocatedBytes),
        runs.Sum(r => r.Gen0),
        runs.Sum(r => r.Gen1),
        runs.Sum(r => r.Gen2),
        TimeSpan.FromTicks(runs.Sum(r => r.GcPause.Ticks)),
        runs[0].Digest);

    private static double MinMs(List<Measurement> runs) => runs.Min(r => r.Elapsed.TotalMilliseconds);

    private static double MaxMs(List<Measurement> runs) => runs.Max(r => r.Elapsed.TotalMilliseconds);
}
