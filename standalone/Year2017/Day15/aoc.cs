var seedA = 722;
var seedB = 354;
var sw = Stopwatch.StartNew();
var part1 = Generator.GetNofMatches(seedA, seedB, 40_000_000);
var part2 = Generator.GetNofMatches(seedA, seedB, 5_000_000, 4, 8);
Console.WriteLine((part1, part2, sw.Elapsed));