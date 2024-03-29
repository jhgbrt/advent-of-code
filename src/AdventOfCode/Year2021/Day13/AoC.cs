namespace AdventOfCode.Year2021.Day13;
using Set = ImmutableHashSet<Coordinate>;

public class AoC202113
{
    static string[] input = Read.InputLines();
    static Grid grid = new Grid(
        from line in input
        let c = Coordinate.TryParse(line)
        where c.HasValue
        select c.Value
    );
    static IEnumerable<Instruction> instructions =
        from line in input
        let i = Instruction.TryParse(line)
        where i.HasValue
        select i.Value;

    public object Part1() => FoldingCycle(grid, instructions).First().Count();
    public object Part2() => FoldingCycle(grid, instructions).Last().ToString().DecodePixels(AsciiFontSize._4x6);

    static IEnumerable<Grid> FoldingCycle(Grid grid, IEnumerable<Instruction> instructions)
    {
        foreach (var (c, value) in instructions)
        {
            grid = grid.Fold(c, value);
            yield return grid;
        }
    }
}


class Grid
{
    Set points;
    (int x, int y) size;
    public Grid(IEnumerable<Coordinate> coordinates)
    {
        points = coordinates.ToImmutableHashSet();
        var max = coordinates.Aggregate((x: 0, y: 0), (p, c) => (Math.Max(p.x, c.x), Math.Max(p.y, c.y)));
        size = (max.x + 1, max.y + 1);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < size.y; y++)
        {
            for (var x = 0; x < size.x; x++)
                sb.Append(points.Contains(new(x, y)) ? '#' : '.');
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public Grid Fold(char c, int v) => new Grid(c switch
    {
        'y' => FoldUp(points, v),
        'x' => FoldLeft(points, v),
        _ => throw new Exception()
    });

    private Set FoldUp(Set coordinates, int v)
        => (from d in Range(1, size.y - v)
            from x in Range(0, size.x)
            where coordinates.Contains(new(x, d + v))
            select (d, x)
            ).Aggregate(coordinates, (c, t) => c.Remove(new(t.x, v + t.d)).Add(new(t.x, v - t.d)));
    private Set FoldLeft(Set coordinates, int v)
        => (from d in Range(1, size.x - v)
            from y in Range(0, size.y)
            where coordinates.Contains(new(d + v, y))
            select (d, y)
            ).Aggregate(coordinates, (c, t) => c.Remove(new(v + t.d, t.y)).Add(new(v - t.d, t.y)));

    internal int Count() => points.Count;
}

record struct Coordinate(int x, int y)
{
    static Regex regex = new Regex(@"^(?<x>\d+),(?<y>\d+)$");
    internal static Coordinate? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Coordinate(int.Parse(match.Groups["x"].ValueSpan), int.Parse(match.Groups["y"].ValueSpan)) : null;
    }
}
record struct Instruction(char c, int v)
{
    static Regex regex = new Regex(@"^fold along (?<c>x|y)=(?<v>\d+)$");
    internal static Instruction? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Instruction(match.Groups["c"].ValueSpan[0], int.Parse(match.Groups["v"].ValueSpan)) : null;

    }
}
