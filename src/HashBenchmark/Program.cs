using HashBenchmark;

var options = Options.Parse(args);

var messages = MessageFactory.Create(options.MessagesCount, options.MessageSize);

Console.WriteLine($"SHA-256 | {options.MessagesCount} messages x {options.MessageSize} B | {options.LoopCounter} loops | .NET {Environment.Version}");

// Warm-up: the first call pays for JIT compilation; run each method once so loop 1 measures hashing only.
OneShotHasher.Hash(messages);

IncrementalHasher.Hash(messages);

Console.WriteLine("Waiting 2 seconds ...");

Console.WriteLine();

await Task.Delay(TimeSpan.FromSeconds(2));

var oneShots = new List<Measurement>();

var incrementals = new List<Measurement>();

for (var loop = 1; loop <= options.LoopCounter; loop++)
{
    var oneShot = Measurement.Run("One-shot", OneShotHasher.Hash, messages);

    var incremental = Measurement.Run("Incremental", IncrementalHasher.Hash, messages);

    oneShots.Add(oneShot);

    incrementals.Add(incremental);

    Report.Loop(loop, options.LoopCounter, oneShot, incremental, options);
}

Report.Summary(oneShots, incrementals, options);
