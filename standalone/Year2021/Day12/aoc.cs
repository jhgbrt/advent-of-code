var input = File.ReadAllLines("input.txt");
var edges = (
    from line in input
    let s = line.Split('-')
    from edge in new[] { (source: s[0], target: s[1]), (source: s[1], target: s[0]) }
    select edge).ToLookup(x => new Node(x.source), x => new Node(x.target));
var START = "start";
var END = "end";
var sw = Stopwatch.StartNew();
var part1 = Count(ImmutableList<Node>.Empty.Add(new Node(START)), 1);
var part2 = Count(ImmutableList<Node>.Empty.Add(new Node(START)), 2);
Console.WriteLine((part1, part2, sw.Elapsed));
int Count(ImmutableList<Node> path, int mode) => path[^1].id == END ? 1 : edges[path[^1]].Aggregate(0, (total, node) => total + (mode, node.id, visited: node.CanVisit(path)) switch
{
    (2, not START, false) => Count(path.Add(node), 1),
    (_, _, true) => Count(path.Add(node), mode),
    _ => 0
});
record struct Node(string id)
{
    public bool CanVisit(IEnumerable<Node> path) => id.Any(char.IsUpper) || !path.Contains(this);
}