namespace AdventOfCode.Year2015.Day18;

public class AoC201518
{
    static string[] lines = Read.InputLines();
    static Grid grid = new Grid((
        from y in Range(0, lines.Length)
        from x in Range(0, lines[y].Length)
        where lines[y][x] == '#'
        select new Coordinate(x, y)
        ).ToHashSet(), lines.Length);

    public object Part1() => Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count();
    public object Part2() => Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count();
}
record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}