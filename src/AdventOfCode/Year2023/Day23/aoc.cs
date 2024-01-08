
namespace AdventOfCode.Year2023.Day23;
public class AoC202323
{
    public AoC202323():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly Grid grid;
    readonly string[] input;
    public AoC202323(string[] input, TextWriter writer)
    {
        this.input = input;
        grid = new Grid(input);
        this.writer = writer;
    }

    public object Part1() => Solve(grid);
    public object Part2() => Solve(grid.With(b =>
    {
        var toclear = b.Where(kv => kv.Value is '>' or '<' or '^' or 'v').Select(kv => kv.Key).ToList();
        foreach (var item in toclear)
        {
            b.Remove(item);
        }
    }));

    int Solve(Grid grid)
    {
    
        var graph = ToGraph(grid);
        var (start, goal) = (
            grid[null, 0].Where(x => x.value == '.').Select(x => x.c).Single(), 
            grid[null, ^1].Where(x => x.value == '.').Select(x => x.c).Single()
            );

        var edgesbysource = graph.Edges.ToLookup(x => x.from);

        return LongestPath(start, goal, [], edgesbysource) ?? 0;
    }

    int? LongestPath(Coordinate node, Coordinate goal, ImmutableHashSet<Coordinate> visited, ILookup<Coordinate, Edge<Coordinate>> graph)
    {
        if (node == goal)
        {
            return 0;
        }
        else if (visited.Contains(node))
        {
            // cycle -> no path
            return null;
        }

        return (
            from source in graph[node]
            select source.distance + LongestPath(source.to, goal, visited.Add(node), graph)
        ).Max();
    }

    Graph<Coordinate> ToGraph(Grid map)
    {
        var nodes = (
            from pos in map.Points()
            orderby pos.y, pos.x
            where map[pos] != '#' && map.Neighbours(pos).Count(d => map[d] != '#') is not 2
            select pos
        ).ToArray();

        var edges = (
            from l in nodes
            from r in nodes
            where l != r
            let distance = Distance(map, l, r)
            where distance.HasValue
            select new Edge<Coordinate>(l, r, distance.Value)
        ).ToImmutableArray();

        return new(edges);
    }

    int? Distance(Grid grid, Coordinate from, Coordinate to)
    {
        var q = new Queue<(Coordinate, int)>();
        q.Enqueue((from, 0));

        var visited = new HashSet<Coordinate> {  };
        while (q.Any())
        {
            var (p, dist) = q.Dequeue();
            visited.Add(p);
            var neighbours = grid[p] switch
            {
                '>' => [p with { x = p.x + 1 }],
                '<' => [p with { x = p.x - 1 }],
                '^' => [p with { y = p.y - 1 }],
                'v' => [p with { y = p.y + 1 }],
                '.' => grid.Neighbours(p).Where(p => grid[p] != '#'),
                '#' => []
            };

            foreach (var n in neighbours)
            {
                if (n == to)
                {
                    return dist + 1;
                }
                else if (grid.Neighbours(n).Count(m => grid[m] != '#') == 2 && !visited.Contains(n))
                {
                    q.Enqueue((n, dist + 1));
                }
            }
        }

        // not connected
        return null;
    }

}

public class AoC202323Tests
{
    private readonly AoC202323 sut;
    private TestWriter writer;
    public AoC202323Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        writer = new TestWriter(output);
        sut = new AoC202323(input, writer);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(94, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(154, sut.Part2());
    }

    
}

class Grid
{

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.y;
    public int Width => endmarker.x;
    public Grid(string[] input, char empty = '.')
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {
    }
    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    internal Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, endmarker.y)
        from x in Range(origin.x, endmarker.x)
        select new Coordinate(x, y);

  
    public IEnumerable<(Coordinate c, char value)> this[Index? x, Index? y]
    {
        get
        {
            if (x.HasValue && y.HasValue)
            {
                var c = x.Value.IsFromEnd ? Width - x.Value.Value : x.Value.Value;
                var r = y.Value.IsFromEnd ? Height - y.Value.Value : y.Value.Value;
                yield return (new(r,c), this[(c, r)]);
            }
            else if (x.HasValue)
            {
                var c = x.Value.IsFromEnd ? Width - x.Value.Value : x.Value.Value;
                foreach (var r in Range(0, Height))
                    yield return (new(c, r), this[c, r]);
            }
            else if (y.HasValue)
            {
                var r = y.Value.IsFromEnd ? Height - y.Value.Value : y.Value.Value;
                foreach (var c in Range(0, Width))
                    yield return (new(c, r), this[c, r]);
            }
            else
            {
                foreach (var r in Range(0, Height))
                    foreach (var c in Range(0, Width))
                        yield return (new(c, r), this[c, r]);
            }
        }
    }

    public IEnumerable<Coordinate> Neighbours(Coordinate p)
        => from d in new[]
            {
                (-1, 0),
                (0, 1),
                (1, 0),
                (0, -1)
            }
           let n = p + d
           where IsValid(p + d)
           select p + d;
    public Grid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new Grid(builder.ToImmutable(), empty, endmarker);
    }

    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < endmarker.x && p.y < endmarker.y;

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < endmarker.y; y++)
        {
            for (int x = origin.x; x < endmarker.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

}


readonly record struct Coordinate(int x, int y)
{
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}
readonly record struct Edge(Coordinate start, Coordinate end, int distance);
readonly record struct Edge<TNode>(TNode from, TNode to, int distance);


class Graph<TNode>
{
    private IReadOnlySet<TNode> _nodes;
    private ImmutableArray<Edge<TNode>> _edges;
    public Graph(IEnumerable<Edge<TNode>> edges)
    {
        _nodes = edges.Select(e => e.from).Concat(edges.Select(e => e.to)).ToImmutableHashSet();
        _edges = edges.ToImmutableArray();
    }
    public IEnumerable<Edge<TNode>> Edges => _edges;
    public IReadOnlySet<TNode> Nodes => _nodes;
    
}