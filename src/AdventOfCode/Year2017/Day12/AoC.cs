namespace AdventOfCode.Year2017.Day12;

public class AoC201712
{
    static string[] input = Read.InputLines();
    static IEnumerable<(int vertex1, int vertex2)> edges = (
                from line in input
                let parts = line.Split("<->").Select(s => s.Trim()).ToArray()
                let vertex1 = int.Parse(parts[0])
                from vertex2 in parts[1].Split(',').Select(int.Parse)
                select (vertex1: vertex1, vertex2: vertex2)
            ).ToArray();
    public object Part1() => new Graph(edges).Count(0);
    public object Part2() => new Graph(edges).SubGraphs().Count;


}

class Graph
{
    private readonly ImmutableList<(int vertex1, int vertex2)> _edges;

    public Graph(IEnumerable<(int, int)> edges)
    {
        _edges = edges.ToImmutableList();
    }

    public IReadOnlyDictionary<int, IReadOnlyCollection<int>> SubGraphs()
    {
        var parents = _edges.ToLookup(x => x.vertex2, x => x.vertex1);
        var subgraphs = ImmutableDictionary<int, IReadOnlyCollection<int>>.Empty;
        foreach (var parent in _edges.Select(x => x.vertex1))
        {
            if (subgraphs.Any(nodes => nodes.Value.Contains(parent))) continue;
            var subgraph = new[] { parent }.ToImmutableHashSet();
            int count;
            do
            {
                count = subgraph.Count;
                subgraph = subgraph.Union(subgraph.SelectMany(c => parents[c]));
            } while (subgraph.Count > count);
            subgraphs = subgraphs.Add(parent, subgraph);
        }
        return subgraphs;

    }

    static void ForEach(ILookup<int, int> nodes, int start, Action<int> action)
    {
        var hashSet = new HashSet<int>();
        VisitAll(nodes, new[] { start }, hashSet, action);
    }
    static void VisitAll(ILookup<int, int> nodes, IEnumerable<int> start, ISet<int> visited, Action<int> action)
    {
        foreach (int v in start)
        {
            if (visited.Contains(v)) continue;
            visited.Add(v);
            action(v);
            VisitAll(nodes, nodes[v], visited, action);
        }
    }

    public int Count(int i)
    {
        var children = _edges.ToLookup(x => x.vertex1, x => x.vertex2);
        int n = 0;
        ForEach(children, 0, _ => n++);
        return n;

    }
}
