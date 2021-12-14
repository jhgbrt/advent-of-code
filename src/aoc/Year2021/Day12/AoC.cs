namespace AdventOfCode.Year2021.Day12;

public class AoC202112
{
    static string[] input = Read.InputLines(typeof(AoC202112));

    static ILookup<Node, Node> edges = (
        from line in input
        let s = line.Split('-')
        from edge in new[] {(source: s[0], target: s[1]), (source: s[1], target: s[0])}
        select edge
        ).ToLookup(x => new Node(x.source), x => new Node(x.target));

    const string START = "start";

    const string END = "end";

    public object Part1() => Count(ImmutableList<Node>.Empty.Add(new Node(START)), 1);
    public object Part2() => Count(ImmutableList<Node>.Empty.Add(new Node(START)), 2);
    static int Count(ImmutableList<Node> path, int mode) => path[^1].id == END
    ? 1
    : edges[path[^1]].Aggregate(0, (total, node) => total + (mode, node.id, visited: node.CanVisit(path)) switch
    {
        (2, not START, false) => Count(path.Add(node), 1),
        (_, _, true) => Count(path.Add(node), mode),
        _ => 0
    });
}
record struct Node(string id)
{
    public bool CanVisit(IEnumerable<Node> path) => id.Any(char.IsUpper) || !path.Contains(this);
}