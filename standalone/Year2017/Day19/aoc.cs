var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = new MazeRunner(input).Traverse().code;
var part2 = new MazeRunner(input).Traverse().steps;
Console.WriteLine((part1, part2, sw.Elapsed));