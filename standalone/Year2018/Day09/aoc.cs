var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(465, 71498);
var part2 = Part2(465, 71498);
Console.WriteLine((part1, part2, sw.Elapsed));