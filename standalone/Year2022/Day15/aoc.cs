var input = File.ReadAllLines("input.txt");
var Y = 2000000;
var max = 4000000;
var lineregex = new Regex("Sensor at (?<sensor>.+): closest beacon is at (?<beacon>.+)", RegexOptions.Compiled);
var pointregex = new Regex(@"x=(?<x>[-\d]+), y=(?<y>[-\d]+)", RegexOptions.Compiled);
var list = (
    from l in input
    let match = lineregex.Match(l)
    let s = pointregex.As<Point>(match.Groups["sensor"].Value)
    let beacon = pointregex.As<Point>(match.Groups["beacon"].Value)
    let sensor = new Sensor(s, s.ManhattanDistance(beacon))
    select (sensor, beacon)).ToArray();
var sw = Stopwatch.StartNew();
var part1 = (
    from x in list
    let sensor = x.sensor
    let intersection = sensor.Intersection(Y)
    where intersection.HasValue
    select intersection.Value).Merge().Sum(r => r.end - r.start);
var part2 = (
    from y in Range(1, max - 1)
    let ranges = (
        from item in list
        let sensor = item.sensor
        let intersection = sensor.Intersection(y)
        where intersection.HasValue
        select intersection.Value).Merge().ToArray()
    where ranges.Length == 2
    select new Point(ranges[0].end + 1, y)).First().TuningFrequency();
Console.WriteLine((part1, part2, sw.Elapsed));
void Draw()
{
    IDictionary<Point, char> map = new Dictionary<Point, char>();
    foreach (var item in list)
    {
        map[item.sensor.position] = 'S';
        map[item.beacon] = 'B';
    }

    foreach (var sensor in list.Select(x => x.sensor))
    {
        foreach (var p in sensor.PointsInRange())
            if (!map.ContainsKey(p))
                map[p] = '#';
    }

    int minX = 0; //map.Keys.Min(x => x.x);
    int maxX = 20; // map.Keys.Max(x => x.x);
    int minY = 0; // map.Keys.Min(x => x.y);
    int maxY = 20; // map.Keys.Max(x => x.y);
    var sb = new StringBuilder();
    for (int i = minX - 3; i < 0; i++)
        sb.Append(' ');
    sb.AppendLine("0    5    1    1    2    2");
    for (int i = minX - 3; i < 0; i++)
        sb.Append(' ');
    sb.AppendLine("012345678901234567890123456789");
    for (int y = minY; y <= maxY; y++)
    {
        sb.Append($"{y:+00;-00}");
        for (int x = minX; x <= maxX; x++)
            sb.Append(map[new(x, y)]);
        sb.AppendLine();
    }

    Console.WriteLine(sb);
}

record Sensor(Point position, int range)
{
    public bool InRange(Point other) => position.ManhattanDistance(other) <= range;
    public (int start, int end)? Intersection(int y)
    {
        var dy = Math.Abs(position.y - y);
        var dx = range - dy;
        if (dx < 0)
            return null;
        return (position.x - dx, position.x + dx);
    }

    public IEnumerable<Point> PointsInRange()
    {
        for (int dx = 0; dx <= range; dx++)
            for (int dy = 0; dy <= range - dx; dy++)
            {
                if (dx == 0 && dy == 0)
                    continue;
                yield return new(position.x + dx, position.y + dy);
                yield return new(position.x + dx, position.y - dy);
                yield return new(position.x - dx, position.y - dy);
                yield return new(position.x - dx, position.y + dy);
            }
    }
}

readonly record struct Point(int x, int y)
{
    public int ManhattanDistance(Point o) => Math.Abs(x - o.x) + Math.Abs(y - o.y);
    public override string ToString() => $"({x},{y})";
    public static Point operator +(Point left, Point right) => new(left.x + right.x, left.y + right.y);
}

static class Extensions
{
    public static long TuningFrequency(this Point p) => p.x * 4000000L + p.y;
    public static IEnumerable<(int start, int end)> Merge(this IEnumerable<(int start, int end)> ranges)
    {
        var q =
            from r in ranges
            orderby r.start, r.end
            select r;
        var enumerator = q.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;
        var (start, end) = enumerator.Current;
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.start > end + 1)
            {
                yield return (start, end);
                (start, end) = enumerator.Current;
            }
            else
            {
                end = Math.Max(end, enumerator.Current.end);
            }
        }

        yield return (start, end);
    }
}