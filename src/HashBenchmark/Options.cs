namespace HashBenchmark;

public sealed record Options(int MessagesCount, int LoopCounter, int MessageSize)
{
    // Usage: --messages-count 1000 --loop-counter 10 --message-size 1024
    public static Options Parse(string[] args)
    {
        var messagesCount = 1000;

        var loopCounter = 10;

        var messageSize = 1024;

        for (var i = 0; i + 1 < args.Length; i += 2)
        {
            var value = int.Parse(args[i + 1]);

            switch (args[i])
            {
                case "--messages-count": messagesCount = value; break;

                case "--loop-counter": loopCounter = value; break;

                case "--message-size": messageSize = value; break;

                default: throw new ArgumentException($"Unknown option '{args[i]}'.");
            }
        }

        return new Options(messagesCount, loopCounter, messageSize);
    }
}
