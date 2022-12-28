var key = "hxtvlmkl";
var sw = Stopwatch.StartNew();
var part1 = Defrag.CountBitsInGrid(key);
var part2 = Defrag.CountRegions(key);
Console.WriteLine((part1, part2, sw.Elapsed));