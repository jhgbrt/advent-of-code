var regex = new Regex(@"(?<name>\w+) can fly (?<speed>\d+) km/s for (?<fly>\d+) seconds, but then must rest for (?<rest>\d+) seconds");
var input = File.ReadAllLines("input.txt");
var maxtime = 2503;
var sw = Stopwatch.StartNew();
var part1 = GetEntries().Select(e => e.GetDistance(maxtime)).Max();
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part2()
{
    var entries = GetEntries();
    var points = entries.ToDictionary(e => e, e => 0);
    var tick = entries.Select(e => (entry: e, distance: 0)).ToList();
    for (int t = 1; t <= maxtime; t++)
    {
        tick = (
            from e in tick
            select (e.entry, e.entry.GetDistance(t))).ToList();
        var winners = (
            from e in tick
            group e by e.distance into g
            orderby g.Key descending
            select g).First();
        foreach (var winner in winners)
            points[winner.entry] += 1;
    }

    return points.Max(x => x.Value);
}

IEnumerable<Entry> GetEntries() =>
    from line in input
    select regex.As<Entry>(line);
record struct Entry(string name, int speed, int fly, int rest)
{
    public int GetDistance(int time)
    {
        var r = time % (fly + rest);
        var n = time / (fly + rest);
        var t = n * fly + Math.Min(fly, r);
        var d = t * speed;
        return d;
    }
}

static partial class AoCRegex
{
    public static T As<T>(this Regex regex, string s, IFormatProvider? provider = null)
        where T : struct
    {
        var match = regex.Match(s);
        if (!match.Success)
            throw new InvalidOperationException($"input '{s}' does not match regex '{regex}'");
        var constructor = typeof(T).GetConstructors().Single();
        var j =
            from p in constructor.GetParameters()
            join m in match.Groups.OfType<Group>() on p.Name equals m.Name
            select Convert.ChangeType(m.Value, p.ParameterType, provider ?? CultureInfo.InvariantCulture);
        return (T)constructor.Invoke(j.ToArray());
    }

    public static int GetInt32(this Match m, string name) => int.Parse(m.Groups[name].Value);
}