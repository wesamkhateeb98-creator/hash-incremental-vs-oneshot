namespace HashBenchmark;

// Prints Markdown to the console AND to output.md, so the report can be opened or copied as a .md file.
public sealed class Report(TextWriter file)
{
    private const string Columns =
        "| Time (ms) | Allocated (B) | Batches/s | Messages/s | MB/s | GC pause (ms) |";

    private const string Alignment =
        "|---:|---:|---:|---:|---:|---:|";

    public void Settings(string[] args, Options options, byte[] digest)
    {
        Line("# Benchmark Report: SHA-256 One-shot vs Incremental");

        Line();

        Line("```bash");

        Line($"dotnet run -c Release -- {string.Join(' ', args)}");

        Line("```");

        Line();

        Line("| Setting | Value |");

        Line("|---|---|");

        Line("| Algorithm | SHA-256 |");

        Line($"| Messages per batch | {options.MessagesCount:N0} |");

        Line($"| Message size | {options.MessageSize:N0} B |");

        Line($"| Batch size | {(long)options.MessagesCount * options.MessageSize:N0} B |");

        Line($"| Loops | {options.LoopCounter} |");

        Line($"| Runtime | .NET {Environment.Version} |");

        Line("| Delay before loops | 2 seconds |");

        Line($"| Digest | `{Convert.ToHexStringLower(digest)}` |");

        Line();
    }

    public void LoopHeader()
    {
        Line("## Per loop");

        Line();

        Line($"| Loop | Method {Columns} Digest match |");

        Line($"|---:|---{Alignment}:---:|");
    }

    public void Loop(int loop, Measurement oneShot, Measurement incremental, Options options)
    {
        var match = oneShot.Digest.AsSpan().SequenceEqual(incremental.Digest) ? "✅" : "❌";

        Line($"| {loop} {Row(oneShot, options)} {match} |");

        Line($"| {loop} {Row(incremental, options)} {match} |");
    }

    public void Summary(List<Measurement> oneShots, List<Measurement> incrementals, Options options)
    {
        var oneShotAverage = Average(oneShots);

        var incrementalAverage = Average(incrementals);

        Line();

        Line($"## Summary (average of {oneShots.Count} loops)");

        Line();

        Line($"| Method {Columns}");

        Line($"|---{Alignment}");

        Line(Row(oneShotAverage, options));

        Line(Row(incrementalAverage, options));

        Line();

        Line($"> GC pause is the **total** over {oneShots.Count} loops; the other columns are averages.");

        Line();

        Line("| Method | Min time (ms) | Max time (ms) |");

        Line("|---|---:|---:|");

        Line($"| One-shot | {MinMs(oneShots):F3} | {MaxMs(oneShots):F3} |");

        Line($"| Incremental | {MinMs(incrementals):F3} | {MaxMs(incrementals):F3} |");

        Line();

        var speedRatio = oneShotAverage.Elapsed.TotalMilliseconds / incrementalAverage.Elapsed.TotalMilliseconds;

        var memoryRatio = (double)oneShotAverage.AllocatedBytes / incrementalAverage.AllocatedBytes;

        Line($"**Result:** Incremental is **{speedRatio:F2}x** the speed of one-shot and allocates **{memoryRatio:F0}x** less memory.");
    }

    private static string Row(Measurement m, Options options)
    {
        var totalBytes = (double)options.MessagesCount * options.MessageSize;

        var messagesPerSecond = m.BatchesPerSecond * options.MessagesCount;

        var megabytesPerSecond = m.BatchesPerSecond * totalBytes / (1024 * 1024);

        return $"| {m.Method} | {m.Elapsed.TotalMilliseconds:F3} | {m.AllocatedBytes:N0} | {m.BatchesPerSecond:N0} | {messagesPerSecond:N0} | {megabytesPerSecond:N0} | {m.GcPause.TotalMilliseconds:F3} |";
    }

    // Average keeps the total GC pause (sum), because "3.5 ms paused in 10 loops" reads better than "0.35".
    private static Measurement Average(List<Measurement> runs) => new(
        runs[0].Method,
        TimeSpan.FromTicks((long)runs.Average(r => r.Elapsed.Ticks)),
        (long)runs.Average(r => r.AllocatedBytes),
        TimeSpan.FromTicks(runs.Sum(r => r.GcPause.Ticks)),
        runs[0].Digest);

    private static double MinMs(List<Measurement> runs) => runs.Min(r => r.Elapsed.TotalMilliseconds);

    private static double MaxMs(List<Measurement> runs) => runs.Max(r => r.Elapsed.TotalMilliseconds);

    private void Line(string text = "")
    {
        Console.WriteLine(text);

        file.WriteLine(text);
    }
}
