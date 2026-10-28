using System.Security.Cryptography;

namespace HashBenchmark;

// One-shot: the whole input must exist as ONE buffer, then it is hashed in a single call.
public static class OneShotHasher
{
    public static byte[] Hash(byte[][] messages)
    {
        var totalLength = 0;

        foreach (var message in messages)
        {
            totalLength += message.Length;
        }

        // The cost of one-shot: a copy of every message in one big array.
        var buffer = new byte[totalLength];

        var offset = 0;

        foreach (var message in messages)
        {
            message.CopyTo(buffer, offset);

            offset += message.Length;
        }

        return SHA256.HashData(buffer);
    }
}
