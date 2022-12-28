using AdventOfCode.Common;

var input = File.ReadAllLines("input.txt");
var numbers = input.Select(int.Parse).ToArray();
var sw = Stopwatch.StartNew();
var part1 = numbers.Aggregate((prev: int.MaxValue, count: 0), (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))).count;
var part2 = numbers.Windowed(3).Select(window => window.Sum()).Aggregate((prev: int.MaxValue, count: 0), (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))).count;
Console.WriteLine((part1, part2, sw.Elapsed));