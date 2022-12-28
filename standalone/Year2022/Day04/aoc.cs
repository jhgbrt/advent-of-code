var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = (
    from line in input
    let pair = RangePair.Parse(line)
    where pair.left.Contains(pair.right) || pair.right.Contains(pair.left)
    select pair).Count();
var part2 = (
    from line in input
    let pair = RangePair.Parse(line)
    where pair.left.Overlaps(pair.right)
    select pair).Count();
Console.WriteLine((part1, part2, sw.Elapsed));
partial record struct Range(int start, int end)
{
    public bool Contains(Range other) => start <= other.start && end >= other.end;
    public bool Overlaps(Range other) => start <= other.end && end >= other.start;
    public static Range Parse(string s) => _r.As<Range>(s);
    static Regex _r = AoCRegex.MyRegex();
    [GeneratedRegex("^(?<start>\\d+)-(?<end>\\d+)$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}

record struct RangePair(Range left, Range right)
{
    static Regex _r = new(@"^(?<left>[^,]+),(?<right>[^,]+)$");
    public static RangePair Parse(string s)
    {
        var match = _r.Match(s);
        return new(Range.Parse(match.Groups["left"].Value), Range.Parse(match.Groups["right"].Value));
    }
}

static partial class AoCRegex
{
    [GeneratedRegex("^(?<start>\\d+)-(?<end>\\d+)$", RegexOptions.Compiled)]
    public static partial Regex MyRegex();
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