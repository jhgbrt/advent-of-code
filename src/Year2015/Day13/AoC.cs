namespace AdventOfCode.Year2015.Day13;

public class AoCImpl : AoCBase
{
    static Regex regex = new Regex(@"(?<first>\w+) would (?<action>lose|gain) (?<amount>\d+) happiness units by sitting next to (?<second>\w+).", RegexOptions.Compiled);
    static string[] input = Read.InputLines(typeof(AoCImpl));
    static readonly ImmutableList<Edge> edges =
        (from line in input
         let match = regex.Match(line)
         let first = match.Groups["first"].Value
         let second = match.Groups["second"].Value
         let value = int.Parse(match.Groups["amount"].Value)
         let action = match.Groups["action"].Value
         select new Edge(first, second, action == "lose" ? -value : value)
        ).ToImmutableList();

    static ImmutableHashSet<string> vertices = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).ToImmutableHashSet();


    public override object Part1() => CalculateScore(edges, vertices);
    public override object Part2() => CalculateScore(edges.AddRange(from v in vertices
                                                                    let edge = new Edge("Jeroen", v, 0)
                                                                    from e in new[] { edge, edge.Reverse() }
                                                                    select e), vertices.Add("Jeroen"));

    static int CalculateScore(IEnumerable<Edge> edges, IReadOnlySet<string> vertices)
    {
        var distances = edges.ToDictionary(e => (e.Source, e.Target), e => e.Points);

        return GetPermutations(vertices, vertices.Count)
            .Select(p =>
            {
                var circle = p.Concat(new[] { p[0] });
                var path = circle.Zip(circle.Skip(1));
                return path.Select(p => distances[(p.First, p.Second)]).Sum() + path.Select(p => distances[(p.Second, p.First)]).Sum();
            }).Max();
    }


    static IEnumerable<T[]> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1
            ? list.Select(t => new[] { t })
            : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(new[] { t2 }).ToArray());

}

record Edge(string Source, string Target, int Points)
{
    public Edge Reverse() => this with { Source = Target, Target = Source };
}
