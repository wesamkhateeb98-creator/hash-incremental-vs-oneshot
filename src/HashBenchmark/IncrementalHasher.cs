using System.Security.Cryptography;

namespace HashBenchmark;

// Incremental: feed the input piece by piece; only the 32 B internal state is kept between calls.
public static class IncrementalHasher
{
    public static byte[] Hash(byte[][] messages)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);

        foreach (var message in messages)
        {
            hash.AppendData(message);
        }

        return hash.GetHashAndReset();
    }
}
