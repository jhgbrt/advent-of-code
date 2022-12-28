using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;

var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input.AsBlocks());
var part2 = Part2(input.AsBlocks());
Console.WriteLine((part1, part2, sw.Elapsed));