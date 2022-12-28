var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input).Last();
var part2 = string.Join(",", Part2(input));
Console.WriteLine((part1, part2, sw.Elapsed));
ImmutableArray<int> Parse(string[] input) => input[0].Split(',').Select(int.Parse).ToImmutableArray();