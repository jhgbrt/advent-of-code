using QuickGraph;
using QuickGraph.Algorithms;

var input = ulong.Parse(File.ReadAllLines("input.txt")[0]);
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = "";
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    (uint x, uint y) start = (1, 1);
    (uint x, uint y) target = (31, 39);
    HashSet<(uint x, uint y)> visited = new()
    {start};
    Stack<((uint x, uint y) pos, uint steps)> stack = new();
    stack.Push((start, 0));
    while (true)
    {
        var (prev, steps) = stack.Pop();
        steps = steps + 1;
        foreach (var next in prev.Neighbours().Where(n => IsSpace(n.x, n.y) && !visited.Contains(n)))
        {
            if (next == target)
            {
                return steps;
            }

            stack.Push((next, steps));
            visited.Add(next);
        }
    }
    //var grid = new Grid(input, 50);
    //return grid.ShortestPath((1,1), (31,39));
}

bool IsSpace(uint x, uint y) => HammingWeight(x * x + 3 * x + 2 * x * y + y + y * y + input) % 2 == 0;
int HammingWeight(ulong i)
{
    i = i - ((i >> 1) & 0x5555555555555555UL);
    i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
    return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
}

static class Ex
{
    public static IEnumerable<(uint x, uint y)> Neighbours(this (uint x, uint y) coordinate)
    {
        var (x, y) = coordinate;
        if (x > 0)
            yield return (x - 1, y);
        if (y > 0)
            yield return (x, y - 1);
        yield return (x + 1, y);
        yield return (x, y + 1);
    }
}

class Grid
{
    ulong input;
    public Grid(ulong input, int size)
    {
        this.input = input;
        for (uint x = 0; x < size; x++)
            for (uint y = 0; y < size; y++)
            {
                var c = this[x, y];
            }
    }

    Dictionary<(uint x, uint y), char> map = new();
    uint Height => map.Keys.Max(x => x.y);
    uint Width => map.Keys.Max(x => x.x);
    public char this[uint x, uint y]
    {
        get
        {
            if (map.TryGetValue((x, y), out char value))
                return value;
            var v = IsSpace(x, y) ? '.' : '#';
            map[(x, y)] = v;
            return v;
        }
    }

    public int ShortestPath((uint x, uint y) start, (uint x, uint y) end) => ShortestPath(Graph(), start, end);
    int ShortestPath(AdjacencyGraph<(uint, uint), Edge<(uint, uint)>> graph, (uint x, uint y) start, (uint x, uint y) end)
    {
        var f = graph.ShortestPathsDijkstra(_ => 1, start);
        return f(end, out var path) ? path.Count() : -1;
    }

    private AdjacencyGraph<(uint, uint), Edge<(uint, uint)>> Graph() => (
        from coordinate in All()
        from n in coordinate.Neighbours()
        where this[n.x, n.y] != '#'
        select new Edge<(uint x, uint y)>(coordinate, n)).ToAdjacencyGraph<(uint, uint), Edge<(uint, uint)>>();
    IEnumerable<(uint x, uint y)> All()
    {
        for (uint y = 0; y < Height; y++)
            for (uint x = 0; x < Width; x++)
                yield return (x, y);
    }

    bool IsSpace(uint x, uint y) => HammingWeight(x * x + 3 * x + 2 * x * y + y + y * y + input) % 2 == 0;
    static int HammingWeight(ulong i)
    {
        i = i - ((i >> 1) & 0x5555555555555555UL);
        i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
        return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
    }

    public override string ToString()
    {
        uint maxX = map.Keys.Max(x => x.x);
        uint maxY = map.Keys.Max(x => x.y);
        var sb = new StringBuilder();
        for (uint y = 0; y <= maxY; y++)
        {
            for (uint x = 0; x <= maxX; x++)
                sb.Append(this[x, y]);
            sb.AppendLine();
        }

        return sb.ToString();
    }
}