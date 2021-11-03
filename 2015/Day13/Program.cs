using System.Text.RegularExpressions;

var regex = new Regex(@"(?<first>\w+) would (?<action>lose|gain) (?<amount>\d+) happiness units by sitting next to (?<second>\w+).", RegexOptions.Compiled);

var lines = File.ReadAllLines("input.txt");

var edges =
    (from line in lines
    let match = regex.Match(line)
    let first = match.Groups["first"].Value
    let second = match.Groups["second"].Value
    let value = int.Parse(match.Groups["amount"].Value)
    let action = match.Groups["action"].Value
    select new Edge(first, second, action == "lose" ? -value : value)
    ).ToList();

var vertices = edges.Select(e => e.Source).Concat(edges.Select(e => e.Target)).ToHashSet();

Console.WriteLine(CalculateScore(edges, vertices));

edges = edges.Concat(
    vertices.Select(v => new Edge("Jeroen", v, 0))
    ).Concat(
    vertices.Select(v => new Edge(v, "Jeroen", 0))
    ).ToList();

vertices.Add("Jeroen");
Console.WriteLine(CalculateScore(edges, vertices));

int CalculateScore(IEnumerable<Edge> edges, HashSet<string> vertices)
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

record Edge(string Source, string Target, int Points);

