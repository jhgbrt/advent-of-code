var input = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();
var sw = Stopwatch.StartNew();
var part1 = input.InvalidNumbers(25).First();
var part2 = input.FindEncryptionWeakness((long)Part1());
Console.WriteLine((part1, part2, sw.Elapsed));