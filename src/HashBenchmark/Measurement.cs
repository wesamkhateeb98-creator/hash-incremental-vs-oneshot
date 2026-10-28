using System.Diagnostics;

namespace HashBenchmark;

public sealed record Measurement(
    string Method,
    TimeSpan Elapsed,
    long AllocatedBytes,
    TimeSpan GcPause,
    byte[] Digest)
{
    // How many whole batches (all messages) could be hashed in one second at this speed.
    public double BatchesPerSecond => 1 / Elapsed.TotalSeconds;

    public static Measurement Run(string method, Func<byte[][], byte[]> hasher, byte[][] messages)
    {
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
            GC.GetTotalPauseDuration() - pauseBefore,
            digest);
    }
}
