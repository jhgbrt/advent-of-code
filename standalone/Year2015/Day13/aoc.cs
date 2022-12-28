var regex = AoCRegex.EdgeRegex();
var input = File.ReadAllLines("input.txt");
var edges = (
    from line in input
    let data = regex.As<Data>(line)
    select new Edge(data.first, data.second, data.action == "lose" ? -data.amount : data.amount)).ToImmutableList();
var vertices = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).ToImmutableHashSet();
var sw = Stopwatch.StartNew();
var part1 = CalculateScore(edges, vertices);
var part2 = CalculateScore(edges.AddRange(
    from v in vertices
    let edge = new Edge("Jeroen", v, 0)
    from e in new[] { edge, edge.Reverse() }
    select e), vertices.Add("Jeroen"));
Console.WriteLine((part1, part2, sw.Elapsed));
int CalculateScore(IEnumerable<Edge> edges, IReadOnlySet<string> vertices)
{
    var distances = edges.ToDictionary(e => (e.Source, e.Target), e => e.Points);
    return GetPermutations(vertices, vertices.Count).Max(p =>
    {
        var circle = p.Concat(p[0]).ToList();
        var path = circle.Zip(circle.Skip(1));
        return path.Select(p => distances[(p.First, p.Second)]).Sum() + path.Select(p => distances[(p.Second, p.First)]).Sum();
    });
}

IEnumerable<T[]> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1 ? list.Select(t => new[] { t }) : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(t2).ToArray());
record Edge(string Source, string Target, int Points)
{
    public Edge Reverse() => this with { Source = Target, Target = Source };
}

record struct Data(string first, string second, string action, int amount);
static partial class AoCRegex
{
    [GeneratedRegex("(?<first>\\w+) would (?<action>lose|gain) (?<amount>\\d+) happiness units by sitting next to (?<second>\\w+).", RegexOptions.Compiled)]
    public static partial Regex EdgeRegex();
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