namespace AdventOfCode.Year2019.Day10;

public class AoC201910
{
    internal static string[] input = Read.InputLines();

    public object Part1() => GetMonitoringPoint(new Grid(input)).n;
    public object Part2()
    {
        var grid = new Grid(input);
        var mp = GetMonitoringPoint(grid).point;
        var vaporized = new List<Coordinate>() { mp };
        while (vaporized.Count < 200)
        {
            var morevaporized =
                from point in grid.NonEmpty()
                where !vaporized.Contains(point)
                let slope = (point - mp).GetReduced()
                group point by slope into g
                let closest = (from p in g
                               orderby p.ManhattanDistance(mp) ascending
                               select p).First()
                orderby closest.Angle(mp)
                select closest;
            vaporized.AddRange(morevaporized);
        }
        var item = vaporized[200];
        return item.x * 100 + item.y;
    }

    (Coordinate point, int n) GetMonitoringPoint(Grid grid) => (
            from item in grid.NonEmpty()
            let n = (from other in grid.NonEmpty()
                     where item != other
                     let slope = other - item
                     select slope.GetReduced()).Distinct().Count()
            select (item, n)
            ).MaxBy(x => x.n);

}

class Grid
{
    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    public int Height => bottomright.y;
    public int Width => bottomright.x;
    public Grid(string[] input, char empty = '.')
    {
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 where input[y][x] != empty
                 select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        bottomright = new(input[0].Length, input.Length);
        this.empty = empty;
    }
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, bottomright.y)
        from x in Range(origin.x, bottomright.x)
        select new Coordinate(x, y);
    public IEnumerable<Coordinate> NonEmpty() => items.Keys;
    public int Count => items.Count;
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < bottomright.y; y++)
        {
            for (int x = origin.x; x < bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";

    public static Slope operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
    public double Angle(Coordinate other) => -Atan2(x - other.x, y - other.y);
}

readonly record struct Slope(int dx, int dy)
{
    public Slope GetReduced()
    {
        var gcd = (int)BigInteger.GreatestCommonDivisor(dx, dy);
        return gcd > 0 ? new(dx / gcd, dy / gcd) : this;
    }

    public override string ToString() => $"({dx},{dy})";
}
