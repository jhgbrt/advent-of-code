using P1 = AdventOfCode.Year2020.Day13.Part1;
using P2 = AdventOfCode.Year2020.Day13.Part2;

var sw = Stopwatch.StartNew();
var part1 = P1.Run();
var part2 = P2.Run();
Console.WriteLine((part1, part2, sw.Elapsed));