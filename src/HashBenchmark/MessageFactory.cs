namespace HashBenchmark;

public static class MessageFactory
{
    // Fixed seed: every run hashes the same bytes, so digests are comparable between runs.
    public static byte[][] Create(int count, int size)
    {
        var random = new Random(42);

        var messages = new byte[count][];

        for (var i = 0; i < count; i++)
        {
            messages[i] = new byte[size];

            random.NextBytes(messages[i]);
        }

        return messages;
    }
}
