using HashBenchmark;

const string OutputFile = "output.md";

// First step: clear output.md, so a failed or interrupted run never leaves the previous report behind.
File.WriteAllText(OutputFile, string.Empty);

var options = Options.Parse(args);

var messages = MessageFactory.Create(options.MessagesCount, options.MessageSize);

// Warm-up: the first call pays for JIT compilation; run each method once so loop 1 measures hashing only.
var digest = OneShotHasher.Hash(messages);

IncrementalHasher.Hash(messages);

// AutoFlush: each loop's rows reach output.md as soon as they are measured.
await using var file = new StreamWriter(OutputFile) { AutoFlush = true };

var report = new Report(file);

report.Settings(args, options, digest);

Console.WriteLine("Waiting 2 seconds ...");

Console.WriteLine();

await Task.Delay(TimeSpan.FromSeconds(2));

report.LoopHeader();

var oneShots = new List<Measurement>();

var incrementals = new List<Measurement>();

for (var loop = 1; loop <= options.LoopCounter; loop++)
{
    var oneShot = Measurement.Run("One-shot", OneShotHasher.Hash, messages);

    var incremental = Measurement.Run("Incremental", IncrementalHasher.Hash, messages);

    oneShots.Add(oneShot);

    incrementals.Add(incremental);

    report.Loop(loop, oneShot, incremental, options);
}

report.Summary(oneShots, incrementals, options);

Console.WriteLine();

Console.WriteLine($"Report saved to {Path.GetFullPath(OutputFile)}");
