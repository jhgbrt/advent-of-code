namespace AdventOfCode.Year2015.Day13;

public partial class AoC201513
{
    static Regex regex = EdgeRegex();
    static string[] input = Read.InputLines();
    static readonly ImmutableList<Edge> edges =
        (from line in input
         let data = regex.As<Data>(line)
         select new Edge(data.first, data.second, data.action == "lose" ? -data.amount : data.amount)
        ).ToImmutableList();

    static ImmutableHashSet<string> vertices = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).ToImmutableHashSet();

    public object Part1() => CalculateScore(edges, vertices);
    public object Part2() => CalculateScore(edges.AddRange(from v in vertices
                                                           let edge = new Edge("Jeroen", v, 0)
                                                           from e in new[] { edge, edge.Reverse() }
                                                           select e), vertices.Add("Jeroen"));

    static int CalculateScore(IEnumerable<Edge> edges, IReadOnlySet<string> vertices)
    {
        var distances = edges.ToDictionary(e => (e.Source, e.Target), e => e.Points);

        return vertices.GetPermutations(vertices.Count)
            .Max(p =>
            {
                var circle = p.Concat(p.First());
                var path = circle.Zip(circle.Skip(1));
                return path.Sum(p => distances[(p.First, p.Second)]) + path.Sum(p => distances[(p.Second, p.First)]);
            });
    }

    [GeneratedRegex("(?<first>\\w+) would (?<action>lose|gain) (?<amount>\\d+) happiness units by sitting next to (?<second>\\w+).", RegexOptions.Compiled)]
    private static partial Regex EdgeRegex();
}

readonly record struct Edge(string Source, string Target, int Points)
{
    public Edge Reverse() => this with { Source = Target, Target = Source };
}
readonly record struct Data(string first, string second, string action, int amount);