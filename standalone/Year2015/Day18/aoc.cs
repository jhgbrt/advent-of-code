var lines = File.ReadAllLines("input.txt");
var grid = new Grid((
    from y in Range(0, lines.Length)
    from x in Range(0, lines[y].Length)
    where lines[y][x] == '#'
    select new Coordinate(x, y)).ToHashSet(), lines.Length);
var sw = Stopwatch.StartNew();
var part1 = Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count();
var part2 = Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count();
Console.WriteLine((part1, part2, sw.Elapsed));
record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}

partial class AoCRegex
{
}