using System.Diagnostics;

namespace HashBenchmark;

public sealed record Measurement(
    string Method,
    TimeSpan Elapsed,
    long AllocatedBytes,
    int Gen0,
    int Gen1,
    int Gen2,
    TimeSpan GcPause,
    byte[] Digest)
{
    // How many whole batches (all messages) could be hashed in one second at this speed.
    public double BatchesPerSecond => 1 / Elapsed.TotalSeconds;

    public static Measurement Run(string method, Func<byte[][], byte[]> hasher, byte[][] messages)
    {
        var gen0Before = GC.CollectionCount(0);

        var gen1Before = GC.CollectionCount(1);

        var gen2Before = GC.CollectionCount(2);

        var pauseBefore = GC.GetTotalPauseDuration();

        var allocatedBefore = GC.GetAllocatedBytesForCurrentThread();

        var stopwatch = Stopwatch.StartNew();

        var digest = hasher(messages);

        stopwatch.Stop();

        var allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        return new Measurement(
            method,
            stopwatch.Elapsed,
            allocated,
            GC.CollectionCount(0) - gen0Before,
            GC.CollectionCount(1) - gen1Before,
            GC.CollectionCount(2) - gen2Before,
            GC.GetTotalPauseDuration() - pauseBefore,
            digest);
    }
}
