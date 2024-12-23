using Net.Code.Graph;
using Net.Code.Graph.Dot;

using Sprache;

namespace AdventOfCode.Year2024.Day23;

public class AoC202423(string[] input, TextWriter writer)
{
    public AoC202423() : this(Read.InputLines(), Console.Out) {}
    Dictionary<string,HashSet<string>> graph = ReadInput(input);
    static Dictionary<string, HashSet<string>> ReadInput(string[] input)
    {
        var graph = new Dictionary<string, HashSet<string>>();
        var edges = from line in input
                let split = line.IndexOf('-')
                from edge in new[]
                {
                    (src: 0..split, dst: (split+1)..),
                    (src: (split+1).., dst: 0..split)
                }
                select (line, edge);

        var lookup = graph.GetAlternateLookup<ReadOnlySpan<char>>();
        foreach (var (line, edge) in edges)
        {
            var span = line.AsSpan();
            var key = span[edge.src];
            var value = span[edge.dst];
            if (!lookup.ContainsKey(key)) lookup[key] = [];
            lookup[key].Add(value.ToString());
        }
        return graph;
    }
    static List<HashSet<string>> FindMaximalCliques(Dictionary<string, HashSet<string>> graph)
    {
        var cliques = new List<HashSet<string>>();
        BronKerbosch([], [.. graph.Keys], [], graph, cliques);
        return cliques;
    }

    static void BronKerbosch(HashSet<string> R, HashSet<string> P, HashSet<string> X, IDictionary<string, HashSet<string>> graph, List<HashSet<string>> cliques)
    {
        if (P.Count == 0 && X.Count == 0)
        {
            cliques.Add([.. R]);
            return;
        }

        foreach (var v in P)
        {
            var neighbors = graph[v];
            BronKerbosch(
                [.. R, v],
                [.. P.Intersect(neighbors)],
                [.. X.Intersect(neighbors)],
                graph,
                cliques
            );
            P.Remove(v);
            X.Add(v);
        }
    }
    public int Part1() => (
            from a in graph.Keys
            from b in graph[a]
            from c in graph[b]
            where graph[a].Contains(c)
            where a[0] == 't' || b[0] == 't' || c[0] == 't'
            let t = (a, b, c).Ordered()
            select t).Distinct().Count();


    public string Part2() => string.Join(",", FindMaximalCliques(graph).MaxBy(cl => cl.Count)!.Order());
}

public class AoC202423Tests
{
    private readonly AoC202423 sut;
    public AoC202423Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202423(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(7, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal("co,de,ka,ta", sut.Part2());
    }
}