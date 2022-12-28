var input = File.ReadAllLines("input.txt");
var Y = input.Length;
var X = input[0].Length;
var graph = (
    from row in input.Select((l, y) => (l, y)) from col in row.l.Select((c, x) => (value: c - '0', x)) select (node: new Node(col.x, row.y), col.value)).ToImmutableDictionary(x => x.node, x => x.value);
var graph2 = (
    from y in Range(0, Y * 5)
    from x in Range(0, X * 5)
    let value = (graph[new(x % X, y % Y)] + y / Y + x / X - 1) % 9 + 1
    select (node: new Node(x, y), value)).ToImmutableDictionary(x => x.node, x => x.value);
var origin = new Node(0, 0);
var target1 = new Node(Y - 1, X - 1);
var target2 = new Node(Y * 5 - 1, X * 5 - 1);
var sw = Stopwatch.StartNew();
var part1 = Dijkstra(graph, origin, target1);
var part2 = Dijkstra(graph2, origin, target2);
Console.WriteLine((part1, part2, sw.Elapsed));
// https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm#Using_a_priority_queue
int Dijkstra(ImmutableDictionary<Node, int> graph, Node source, Node target)
{
    var queue = new PriorityQueue<Node, int>();
    var costs = new Dictionary<Node, int> { [source] = 0 };
    queue.Enqueue(source, 0);
    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        var updates =
            from next in current.Neighbours(target.x, target.y)
            let cost = costs[current] + graph[next]
            where cost < costs.GetValueOrDefault(next, int.MaxValue)
            select (next, cost);
        foreach (var (next, cost) in updates)
        {
            queue.Enqueue(next, cost);
            costs[next] = cost;
        }
    }

    return costs[target];
}

record Node(int x, int y)
{
    public IEnumerable<Node> Neighbours(int X, int Y)
    {
        if (x > 0)
            yield return this with { x = x - 1 };
        if (y > 0)
            yield return this with { y = y - 1 };
        if (x < X)
            yield return this with { x = x + 1 };
        if (y < Y)
            yield return this with { y = y + 1 };
    }
}