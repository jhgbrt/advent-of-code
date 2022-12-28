using P1 = AdventOfCode.Year2020.Day23.Part1.Runner;
using P2 = AdventOfCode.Year2020.Day23.Part2.Runner;

var sw = Stopwatch.StartNew();
var part1 = P1.Run();
var part2 = P2.Run();
Console.WriteLine((part1, part2, sw.Elapsed));