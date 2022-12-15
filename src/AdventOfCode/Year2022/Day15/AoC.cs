using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day15;
public class AoC202215
{
    //static string[] input = Read.SampleLines(); int Y = 10; int factor = 20;
    static string[] input = Read.InputLines(); int Y = 2000000; int factor = 4000000;
    public object Part1()
    {
        var q = (
            from l in input
            let match = lineregex.Match(l)
            let s = pointregex.As<Point>(match.Groups["sensor"].Value)!.Value
            let beacon = pointregex.As<Point>(match.Groups["beacon"].Value)!.Value
            let sensor = new Sensor(s, s.Distance(beacon))
            select (sensor, beacon)
            ).ToArray();


        IDictionary<Point, char> map = new Dictionary<Point, char>();
        foreach (var item in q)
        {
            map[item.sensor.position] = 'S';
            map[item.beacon] = 'B';
        }

        //foreach (var sensor in q.Select(x => x.sensor).Skip(8).Take(1))
        //{
        //    foreach (var p in sensor.position.Within(sensor.range))
        //        if (!map.ContainsKey(p)) map[p] = '#';
        //}

        //Console.WriteLine(new Grid(map));

        var minX = q.Min(p => Math.Min(p.beacon.x, p.sensor.position.x));
        var maxX = q.Max(p => Math.Max(p.beacon.x, p.sensor.position.x));
        var minY = q.Min(p => Math.Min(p.beacon.y, p.sensor.position.y));
        var maxY = q.Max(p => Math.Max(p.beacon.y, p.sensor.position.y));
        var maxRange = q.Max(p => p.sensor.range);
        Console.WriteLine($"{(minX, maxX, minY, maxY)}");

        var query = from x in Range(minX - maxRange, maxX - minX + maxRange)
                    let p = new Point(x, Y)
                    let sensors = (
                        from item in q
                        let sensor = item.sensor
                        where sensor.InRange(p)
                        select sensor
                        )
                    where sensors.Any() && !map.ContainsKey(p)
                    select (p, sensors);

        //foreach (var item in query)
        //    Console.WriteLine($"{item.p} -> {string.Join(",", item.sensors)}");


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
            sb.Append($"{y:0000} ");
            for (int x = minX; x <= maxX; x++)
                sb.Append(this[new(x, y)]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}