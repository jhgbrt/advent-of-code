namespace AdventOfCode.Year2015.Day18;

partial class AoC
{
    static string[] lines = File.ReadAllLines("input.txt");
    static Grid grid = new Grid((
        from y in Enumerable.Range(0, lines.Length)
        from x in Enumerable.Range(0, lines[y].Length)
        where lines[y][x] == '#'
        select new Coordinate(x, y)
        ).ToHashSet(), lines.Length);

    internal static Result Part1() => Run(() => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count());
    internal static Result Part2() => Run(() => Enumerable.Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count());
}
record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}