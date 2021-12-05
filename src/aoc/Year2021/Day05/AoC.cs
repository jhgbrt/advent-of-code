namespace AdventOfCode.Year2021.Day05;

public class AoC202105 : AoCBase
{
    static string[] input = Read.SampleLines(typeof(AoC202105));
    ImmutableArray<Line> lines = input.Select(Line.Parse).ToImmutableArray();
    public override object Part1()
    {
        var grid = new Grid();

        var straighlines = from line in lines
                           where line.@from.x == line.to.x || line.@from.y == line.to.y
                           select line;

        foreach (var line in lines)
        {
            grid.AddLine(line);
        }


        return grid.Overlaps;
    }
    public override object Part2() => -1;
}

record Line(Point from, Point to)
{
    static Regex regex = new Regex(@"(?<x1>\d+),(?<y1>\d+) -> (?<x2>\d+),(?<y2>\d+)");
    internal static Line Parse(string s)
    {
        var match = regex.Match(s);
        Point p1 = new (int.Parse(match.Groups["x1"].Value), int.Parse(match.Groups["y1"].Value));
        Point p2 = new (int.Parse(match.Groups["x2"].Value), int.Parse(match.Groups["y2"].Value));

        return new Line(p1, p2);
    }
    internal bool IsHorizontal => from.y == to.y;
    internal bool IsVertical => from.x == to.x;

}
record Point(int x, int y)
{
    internal Point NextHorizontal() => this with { x = x + 1 };
    internal Point NextVertical() => this with { y = y + 1 };

    public static explicit operator bool <(Point p1, Point p2) => p1.x + p1.y < p2.x + p2.y; 

}

class Grid
{
    Dictionary<Point, int> _points = new();
    public void AddLine(Line line)
    {
        Console.WriteLine($"adding line: {line}");
        switch (line)
        {
            case { IsHorizontal: true }:
                for (var p = line.from; p.x <= line.to.x; p = p.NextHorizontal())
                {
                    Console.WriteLine($"adding point: {p}");
                    if (!_points.ContainsKey(p)) _points[p] = 0;
                    _points[p]++;
                }
                break;
            case { IsVertical: true }:
                for (var p = line.from; p.y <= line.to.y; p = p.NextVertical())
                {
                    Console.WriteLine($"adding point: {p}");
                    if (!_points.ContainsKey(p)) _points[p] = 0;
                    _points[p]++;
                }
                break;
        }
    }

    public int Overlaps => _points.Values.Where(i => i >= 2).Count();
}