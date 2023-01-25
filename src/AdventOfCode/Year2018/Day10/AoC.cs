using AdventOfCode.Year2016.Day08;

namespace AdventOfCode.Year2018.Day10;

public class AoC201810
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);

    public static string Part1(string[] input) => ToGrid(input).FindGridWithLowestHeight().Decode();

    public static int Part2(string[] input) => ToGrid(input).FindGridWithLowestHeight().Ticks;

    internal static Grid ToGrid(string[] input) => new Grid(from s in input select Point.Parse(s));

}

class Grid
{
    Point[] _points;

    public Grid(IEnumerable<Point> points, int ticks = 0)
    {
        _points = points.ToArray();
        Ticks = ticks;
    }

    public Grid Move(int ticks) => new Grid(_points.Select(p => p.Move(ticks)), Ticks + ticks);

    int MinX => _points.Min(p => p.X);
    int MaxX => _points.Max(p => p.X);
    int MinY => _points.Min(p => p.Y);
    int MaxY => _points.Max(p => p.Y);
    public int Ticks { get; }
    public int Width => MaxX - MinX + 1;
    public int Height => MaxY - MinY + 1;

    public override string ToString()
    {
        var grid = Enumerable.Range(0, Height)
            .Select(i => Enumerable.Repeat('.', Width).ToArray())
            .ToArray();

        foreach (var point in _points)
        {
            grid[point.Y - MinY][point.X - MinX] = '#';
        }

        return string.Join(Environment.NewLine, grid.Select(a => new string(a)));
    }
    public string Decode()
    {
        return this.ToString().DecodePixels(AsciiFontSize._6x10, spacing: 2);
    }
}

static class Ex
{
    public static Grid FindGridWithLowestHeight(this Grid grid) => grid.KeepMoving().Where(g => g.Height < g.Move(1).Height).First();
    static IEnumerable<Grid> KeepMoving(this Grid grid)
    {
        while (true)
        {
            grid = grid.Move(1);
            yield return grid;
        }
    }
}

readonly record struct Point(int X, int Y, int Vx, int Vy)
{
    static Regex regex = new Regex(
        @"position=\<\s*(?<x>-?\d+),\s*(?<y>-?\d+)\> velocity=\<\s*(?<dx>-?\d+),\s*(?<dy>-?\d+)\>",
        RegexOptions.Compiled);

    public static Point Parse(string s)
    {
        var result = regex.Match(s);
        return new Point(
            new[] { "x", "y", "dx", "dy" }.Select(g => int.Parse(result.Groups[g].Value)).ToArray()
            );
    }
    private Point(int[] ints) : this(ints[0], ints[1], ints[2], ints[3]) { }

    public Point Move(int seconds) => new Point(X + seconds * Vx, Y + seconds * Vy, Vx, Vy);

    public override string ToString() => (X, Y, Vx, Vy).ToString();
}
