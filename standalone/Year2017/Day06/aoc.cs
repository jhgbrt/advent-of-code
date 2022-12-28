var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).steps;
var part2 = Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).loopSize;
Console.WriteLine((part1, part2, sw.Elapsed));