var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = new Accumulator().Decode(input, 8, false);
var part2 = new Accumulator().Decode(input, 8, true);
Console.WriteLine((part1, part2, sw.Elapsed));