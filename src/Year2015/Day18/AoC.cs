namespace AdventOfCode.Year2015.Day18;

public class AoCImpl : AoCBase
{
    static string[] lines = Read.InputLines(typeof(AoCImpl));
    static Grid grid = new Grid((
        from y in Enumerable.Range(0, lines.Length)
        from x in Enumerable.Range(0, lines[y].Length)
        where lines[y][x] == '#'
        select new Coordinate(x, y)
        ).ToHashSet(), lines.Length);

    public override object Part1() => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count();
    public override object Part2() => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count();
}
record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}