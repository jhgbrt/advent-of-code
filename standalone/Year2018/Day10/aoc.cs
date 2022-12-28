var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
Grid ToGrid(string[] input) => new Grid(
    from s in input
    select Point.Parse(s));