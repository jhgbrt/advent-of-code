using AdventOfCode.Common;
using AdventOfCode.Year2015.Day21;
using AdventOfCode.Year2022.Day02;

namespace AdventOfCode.Year2022.Day15;
public class AoC202215
{
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    int Y = 10;
    //int Y = 2000000;

    static Regex regex = new(@"Sensor at (?<sensor>.+): closest beacon is at (?<beacon>.*)", RegexOptions.Compiled);
    static Regex pointrx = new(@"x=(?<x>[-\d]+), y=(?<y>[-\d]+)");
    public object Part1()
    {
        var q = (
            from line in sample
            let m = regex.Match(line)
            let sensor = pointrx.As<Point>(m.Groups["sensor"].Value)!.Value
            let beacon = pointrx.As<Point>(m.Groups["beacon"].Value)!.Value
            select (sensor, beacon, range: sensor.Distance(beacon))).ToList();

        var minX = q.Min(x => Math.Min(x.sensor.x, x.beacon.x));
        var maxX = q.Max(x => Math.Max(x.sensor.x, x.beacon.x));
        var minY = q.Min(x => Math.Min(x.sensor.y, x.beacon.y));
        var maxY = q.Max(x => Math.Max(x.sensor.y, x.beacon.y));

        foreach (var x in from p in Range(minX, maxX - minX).Select(x => new Point(x, Y))
                          where q.Any(item => item.sensor.Distance(p) <= item.range)
                          select p)
            Console.WriteLine(x);

        return (from p in Range(minX, maxX - minX).Select(x => new Point(x, Y))
                where q.Any(item => item.sensor.Distance(p) <= item.range)
                select p).Count() - 1;
    }

    public object Part2() => "";


        return query.Count();
}

    static Regex lineregex = new Regex("Sensor at (?<sensor>.+): closest beacon is at (?<beacon>.+)", RegexOptions.Compiled);
    static Regex pointregex = new Regex(@"x=(?<x>[-\d]+), y=(?<y>[-\d]+)", RegexOptions.Compiled);

    // ......####B######################..
    public object Part2() => "";
}
record Sensor(Point position, int range)
{
    public bool InRange(Point other) => position.Distance(other) <= range;
}

static class Ex
{
    public static IEnumerable<Point> WithinDistance(this Point p, int d)
    {
        for (int dx = 0; dx <= d; dx++)
            for (int dy = 0; dy <= (d - dx); dy++)
            {
                yield return new Point(p.x + dx, p.y + dy);
                yield return new Point(p.x + dx, p.y - dy);
                yield return new Point(p.x - dx, p.y - dy);
                yield return new Point(p.x - dx, p.y + dy);
            }
    }
}

class Grid
{
    private IDictionary<Point, char> tiles;

    private readonly int MinY;
    private readonly int MaxY;
    private readonly int MinX;
    private readonly int MaxX;

    public Grid(IDictionary<Point, char> tiles)
    {
        this.tiles = tiles;
        this.MinY = tiles.Keys.Min(x => x.y);
        this.MaxY = tiles.Keys.Max(x => x.y);
        this.MinX = tiles.Keys.Min(x => x.x);
        this.MaxX = tiles.Keys.Max(x => x.x);
    }

    public IEnumerable<Point> Sensors => tiles.Where(x => x.Value == 'S').Select(x => x.Key);

    char this[Point c]
    {
        get => tiles.TryGetValue(c, out char value) ? value : '.';
        set { tiles[c] = value; }
    }

    public IEnumerable<Point> GetPointsAtY(int y)
    {
        for (int x = MinX; x <= MaxX; x++)
        {
            yield return new(x, y);
        }
    }

    public override string ToString()
    {
        int minX = map.Keys.Min(x => x.x);
        int maxX = map.Keys.Max(x => x.y);
        int minY = map.Keys.Min(x => x.x);
        int maxY = map.Keys.Max(x => x.y);
        var sb = new StringBuilder();
        for (int y = MinY; y <= MaxY; y++)
        {
            for (int x = MinX; x <= MaxX; x++)
                sb.Append(this[new(x, y)]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
