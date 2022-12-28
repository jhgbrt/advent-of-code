var input = File.ReadAllLines("input.txt").Select(int.Parse).ToArray();
var sw = Stopwatch.StartNew();
var part1 = Jumps.CalculateJumps(input, v => 1);
var part2 = Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1);
Console.WriteLine((part1, part2, sw.Elapsed));