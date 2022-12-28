var sw = Stopwatch.StartNew();
var part1 = new GarbageProcessor().ProcessFile("input.txt").Score;
var part2 = new GarbageProcessor().ProcessFile("input.txt").GarbageCount;
Console.WriteLine((part1, part2, sw.Elapsed));