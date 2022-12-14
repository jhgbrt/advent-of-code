using Net.Code.AdventOfCode.Toolkit;
await AoC.RunAsync(args);

Console.WriteLine($"GC collections: Gen 0: {GC.CollectionCount(0)}");
Console.WriteLine($"GC collections: Gen 1: {GC.CollectionCount(1)}");
Console.WriteLine($"GC collections: Gen 2: {GC.CollectionCount(2)}");
var info = GC.GetGCMemoryInfo();
Console.WriteLine($"GC generation: {info.Generation}");
Console.WriteLine($"GC pause time percentage: {info.PauseTimePercentage}%");
Console.WriteLine($"Fragmented bytes: {info.FragmentedBytes}");
Console.WriteLine($"Heap size bytes: {info.HeapSizeBytes}");
Console.WriteLine($"High memoryload threshold bytes: {info.HighMemoryLoadThresholdBytes}");
Console.WriteLine($"memoryload load bytes: {info.MemoryLoadBytes}");
Console.WriteLine($"promoted bytes: {info.PromotedBytes}");
Console.WriteLine($"total available memory bytes: {info.TotalAvailableMemoryBytes}");
Console.WriteLine($"Committed bytes: {info.TotalCommittedBytes}");

