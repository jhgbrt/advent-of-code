namespace AdventOfCode.Year2021.Day13;

public class AoC202113 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202113));
    static Regex coordinateRegex = new Regex(@"^(?<x>\d+),(?<y>\d+)$");
    static Regex foldRegex = new Regex(@"^fold along (?<coordinate>x|y)=(?<value>\d+)$");
    static Grid grid = new Grid(
        from line in input
        let match = coordinateRegex.Match(line)
        where match.Success
        select new Coordinate(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value))
    );
    static IEnumerable<(char, int)> folding = from line in input let match = foldRegex.Match(line) where match.Success select (match.Groups["coordinate"].Value[0], int.Parse(match.Groups["value"].Value));

    public override object Part1() => FoldingCycle().First().Count();
    public override object Part2() => FoldingCycle().Last();

    IEnumerable<Grid> FoldingCycle()
    {
        foreach (var (c, value) in folding)
        {
            grid = grid.Fold(c, value);
            yield return grid;
        }

    }
}


class Grid
{
    ImmutableHashSet<Coordinate> coordinates;
    int MaxX;
    int MaxY;
    public Grid(IEnumerable<Coordinate> coordinates)
    {
        this.coordinates = coordinates.ToImmutableHashSet();
        MaxX = coordinates.MaxBy(c => c.x)!.x;
        MaxY = coordinates.MaxBy(c => c.y)!.y;
    }

    public override string ToString()
    {
        
        var sb = new StringBuilder();
        sb.AppendLine((MaxX, MaxY).ToString());
        for (var y = 0; y <= MaxY; y++)
        {
            for (var x = 0; x <= MaxX; x++)
            {
                sb.Append(coordinates.Contains(new(x, y)) ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public Grid Fold(char c, int v) => new Grid(c switch
    {
        'y' => FoldUp(coordinates, v),
        'x' => FoldLeft(coordinates, v),
        _ => throw new Exception()
    });

    private ImmutableHashSet<Coordinate> FoldUp(ImmutableHashSet<Coordinate> coordinates, int v)
    {
        var q = from d in Range(1, MaxY - v + 1)
                from x in Range(0, MaxX + 1)
                where coordinates.Contains(new(x, d + v))
                select (src: new Coordinate(x, v + d), to: new Coordinate(x, v - d));

        foreach (var c in q)
            coordinates = coordinates.Remove(c.src).Add(c.to);
        return coordinates;
    }
    private ImmutableHashSet<Coordinate> FoldLeft(ImmutableHashSet<Coordinate> coordinates, int v)
    {
        var q = from d in Range(1, MaxX - v + 1)
                from y in Range(0, MaxY + 1)
                where coordinates.Contains(new(d + v, y))
                select (src: new Coordinate(v+ d, y), to: new Coordinate(v - d, y));

        foreach (var c in q)
            coordinates = coordinates.Remove(c.src).Add(c.to);
        return coordinates;

    }

    internal int Count()
    {
        return coordinates.Count;
    }
}

record Coordinate(int x, int y);

static class AsciiLetters
{
    public static string Decode(string s, int size)
    {
        var lines = Lines(s).ToArray();

        var chunks = lines.Select(l => l.Chunk(size).ToArray());
        return "";

    }
    private static IEnumerable<string> Lines(string s)
    {
        var reader = new StringReader(s);
        while (reader.Peek() >= 0)
            yield return reader.ReadLine()!;
    }

    public const string E = @"
####
#...
###.
#...
#...
####
";
    public const string P = @"
###.
#..#
#..#
###.
#...
#...
";
    public const string U = @"
#..#
#..#
#..#
#..#
#..#
.##.
";
    public const string L = @"
#...
#...
#...
#...
#...
####
";
    public const string B = @"
###.
#..#
###.
#..#
#..#
###.
";
    public const string R = @"
###.
#..#
#..#
###.
#.#.
#..#
";
}