using static AdventOfCode.Year2017.Day03.Spiral;

var input = 265149;
var sw = Stopwatch.StartNew();
var part1 = DistanceToOrigin(input);
var part2 = SpiralValues().SkipWhile(i => i.value <= input).First().value;
Console.WriteLine((part1, part2, sw.Elapsed));