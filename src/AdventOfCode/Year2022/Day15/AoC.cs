using AdventOfCode.Common;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Year2022.Day15;
public class AoC202215
{
    static string[] input = Read.SampleLines(); int Y = 10; int max = 20;
    //static string[] input = Read.InputLines(); int Y = 2000000; int max = 4000000;

    static Regex lineregex = new Regex("Sensor at (?<sensor>.+): closest beacon is at (?<beacon>.+)", RegexOptions.Compiled);
    static Regex pointregex = new Regex(@"x=(?<x>[-\d]+), y=(?<y>[-\d]+)", RegexOptions.Compiled);

    static (Sensor sensor, Point beacon)[] list = (
        from l in input
        let match = lineregex.Match(l)
        let s = pointregex.As<Point>(match.Groups["sensor"].Value)!.Value
        let beacon = pointregex.As<Point>(match.Groups["beacon"].Value)!.Value
        let sensor = new Sensor(s, s.Distance(beacon))
        select (sensor, beacon)
        ).ToArray();

    static int minX = list.Min(p => Math.Min(p.beacon.x, p.sensor.position.x));
    static int maxX = list.Max(p => Math.Max(p.beacon.x, p.sensor.position.x));
    static int minY = list.Min(p => Math.Min(p.beacon.y, p.sensor.position.y));
    static int maxY = list.Max(p => Math.Max(p.beacon.y, p.sensor.position.y));
    static int maxRange = list.Max(p => p.sensor.range);

    private void Draw()
    {
        IDictionary<Point, char> map = new Dictionary<Point, char>();
        foreach (var item in list)
        {
            map[item.sensor.position] = 'S';
            map[item.beacon] = 'B';
        }

        foreach (var sensor in list.Select(x => x.sensor))
        {
            foreach (var p in sensor.position.Within(sensor.range))
                if (!map.ContainsKey(p)) map[p] = '#';
        }

        Console.WriteLine(new Grid(map));
    }

    public object Part1()
    {

        IDictionary<Point, char> map = new Dictionary<Point, char>();
        foreach (var item in list)
        {
            map[item.sensor.position] = 'S';
            map[item.beacon] = 'B';
        }
        Draw();

        var ranges = from x in list
                     let sensor = x.sensor
                     orderby sensor.position.x, sensor.position.y
                     let intersection = sensor.Intersection(Y)
                     where intersection.HasValue
                     select intersection.Value;

        var result = Merge(ranges).Sum(r => r.end - r.start);
        Console.WriteLine(result);
        Console.WriteLine(string.Join("//", ranges));
        Console.WriteLine(string.Join("//", Merge(ranges)));

        var query = from x in Range(minX - maxRange, maxX - minX + maxRange)
                    let p = new Point(x, Y)
                    let sensors = (
                        from item in list
                        let sensor = item.sensor
                        where sensor.InRange(p)
                        select sensor
                        )
                    where sensors.Any() && !map.ContainsKey(p)
                    select p;

        Console.WriteLine(string.Join("//", query));

        return query.Count();
    }

    IEnumerable<(int start, int end)> Merge(IEnumerable<(int start, int end)> source)
    {
        var q = from r in source
                orderby r.start, r.end
                select r;

        var enumerator = q.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var (start, end) = enumerator.Current;

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.start > end)
            {
                yield return (start, end);
                (start, end) = enumerator.Current;
            }
            else
            {
                end = enumerator.Current.end;
            }
        }
        yield return (start, end);
    }


    public long Part2()
    {
        IDictionary<Point, char> map = new Dictionary<Point, char>();
        foreach (var item in list)
        {
            map[item.sensor.position] = 'S';
            map[item.beacon] = 'B';
        }

        var sensors = from item in list
                      let sensor = item.sensor
                      orderby sensor.position.x, sensor.position.y
                      select sensor;


        Console.WriteLine("created grid");
        foreach (var sensor in list.Select(x => x.sensor))
        {
            foreach (var p in sensor.position.Within(sensor.range))
                if (!map.ContainsKey(p)) map[p] = '#';
        }
        Console.WriteLine("filled grid");



        var query = from x in Range(0, max)
                    from y in Range(0, max)
                    let p = new Point(x, y)
                    where !map.ContainsKey(p)
                    select p;
        Console.WriteLine(string.Join(",", query));
        var result = query.Single();
        return result.x * 4000000L + result.y;
    }
}

record Sensor(Point position, int range)
{
    public bool InRange(Point other) => position.Distance(other) <= range;

    public (int start, int end)? Intersection(int y)
    {
        var dy = Math.Abs(position.y - y);
        var dx = range - dy;
        if (dx < 0) return null;
        return (position.x - dx, position.x + dx);
    }

}

static class Ex
{
    public static int Distance(this Point p, Point o) => Math.Abs(p.x - o.x) + Math.Abs(p.y - o.y);
    public static IEnumerable<Point> Within(this Point p, int distance)
    {
        for (int dx = 0; dx <= distance; dx++)
            for (int dy = 0; dy <= distance - dx; dy++)
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
    private IDictionary<Point, char> map;

    public Grid(IDictionary<Point,char> map)
    {
        this.map = map;
    }

    char this[Point c]
    {
        get => map.TryGetValue(c, out char value) ? value : '.';
        set { map[c] = value; }
    }


    public override string ToString()
    {
        int minX = map.Keys.Min(x => x.x);
        int maxX = map.Keys.Max(x => x.y);
        int minY = map.Keys.Min(x => x.x);
        int maxY = map.Keys.Max(x => x.y);
        var sb = new StringBuilder();
        for (int y = minY; y <= maxY; y++)
        {
            sb.Append($"{y:00}");
            for (int x = minX; x <= maxX; x++)
                sb.Append(this[new(x, y)]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}