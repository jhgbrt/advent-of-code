var sw = Stopwatch.StartNew();
var part1 = Driver.Part1(File.ReadAllLines("input.txt"));
var part2 = Driver.Part2(Read.InputLines());
Console.WriteLine((part1, part2, sw.Elapsed));