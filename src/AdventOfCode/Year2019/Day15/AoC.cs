using Net.Code.Graph;
using Net.Code.Graph.Algorithms;

namespace AdventOfCode.Year2019.Day15;
public class AoC201915
{
    public AoC201915():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly IntCode machine;
    readonly Grid grid;
    Coordinate target;
    public AoC201915(string[] input, TextWriter writer)
    {
        this.writer = writer;
        var program = input.First().Split(',').Select(long.Parse).ToArray();
        machine = new IntCode(program);
        grid = new Grid();
        FillGrid(grid, Coordinate.Origin, Direction.N);
    }
    static readonly Direction[] directions = [Direction.N, Direction.S, Direction.W, Direction.E];
    public int Part1()
    {
        var vertices = grid.Keys;
        var edges = from vertex in grid.Keys
                    from direction in directions
                    let next = vertex + direction
                    where grid.Contains(next) && grid[next] != '#'
                    select Edge.Create(vertex, next, 1);

        var graph = GraphBuilder.Create<Coordinate, int>()
            .AddVertices(vertices)
            .AddEdges(edges)
            .BuildGraph();

        var result = Dijkstra.ShortestPaths(graph, Coordinate.Origin);
        return result.GetPath(target).Count();
    }
    void FillGrid(Grid grid, Coordinate pos, Direction direction)
    {

        foreach (var d in directions)
        {
            var next = pos + d;
            if (!grid.Contains(next))
            {
                var result = machine.Run((int)d) switch
                {
                    0 => '#',
                    1 => '.',
                    2 => 'O',
                };
                grid[next] = result;
                if (result != '#')
                {
                    if (result == 'O')
                    {
                        target = next;
                    }
                    FillGrid(grid, next, d);
                }
            }
        }
        if (pos != Coordinate.Origin)
        {
            var reverse = direction switch
            {
                Direction.N => Direction.S,
                Direction.E => Direction.W,
                Direction.S => Direction.N,
                Direction.W => Direction.E,
            };
            machine.Run((int)reverse);
            pos = pos + reverse;
        }
    }

    public int Part2() => Spread(grid, target, 0, 0);

    int Spread(Grid grid, Coordinate position, int time, int max)
    {
        var neighbours = from d in directions
                         let next = position + d
                         where grid[next] == '.'
                         select next;

        foreach (var n in neighbours)
        {
            grid[n] = 'O';
            max = Spread(grid, n, time + 1, max);
        }
        return Max(time, max);
    }
}

public class AoC201915Tests
{
    private readonly AoC201915 sut;
    public AoC201915Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC201915(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(266, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(274, sut.Part2());
    }
}

enum Direction : int { N = 1, E = 4, S = 2, W = 3}
readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate c, Direction d) => d switch
    {
        Direction.N => c with { y = c.y + 1 },
        Direction.E => c with { x = c.x + 1 },
        Direction.S => c with { y = c.y - 1 },
        Direction.W => c with { x = c.x - 1 },
    };
    public static Coordinate operator -(Coordinate c, Direction d) => c + d switch
    {
        Direction.N => Direction.S,
        Direction.E => Direction.W,
        Direction.S => Direction.N,
        Direction.W => Direction.E
    };
}
class Grid
{
    readonly Dictionary<Coordinate, char> items = [];
    readonly char unknown = 'U';
    public char this[Coordinate p]
    {
        get {
            return items.TryGetValue(p, out var c) ? c : unknown;
        }
        set {
            items[p] = value;
        }
    }

    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];
    public IEnumerable<Coordinate> Keys => items.Keys;
    private Coordinate topleft => new(items.Keys.Min(x => x.x), items.Keys.Min(x => x.y));
    private Coordinate bottomright => new(items.Keys.Max(x => x.x), items.Keys.Max(x => x.y));

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = topleft.y; y <= bottomright.y; y++)
        {
            for (int x = topleft.x; x <= bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

  
    public int Count() => items.Count;
    public bool Contains(Coordinate position) => items.ContainsKey(position);
}