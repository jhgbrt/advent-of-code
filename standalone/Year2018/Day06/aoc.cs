var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
IEnumerable<(int x, int y)> Grid(int maxX, int maxY) =>
    from x in Enumerable.Range(0, maxX) from y in Enumerable.Range(0, maxY) select (x, y);