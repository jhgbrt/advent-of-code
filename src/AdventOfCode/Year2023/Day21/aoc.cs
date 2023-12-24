namespace AdventOfCode.Year2023.Day21;
public class AoC202321
{
    public AoC202321():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    private Grid grid;
    private Coordinate start;
    public AoC202321(string[] input, TextWriter writer)
    {
        grid = new Grid(input);
        start = grid.Find('S');
        grid = grid.With(g => g[start] = '.');
        this.writer = writer;
    }

    public long Part1() => Part1(64);
    internal long Part1(int steps) => Solve(grid, start, [steps]).First();
    public long Part2() => Part2(26501365);
    public long Part2(int steps) => -1;

    internal static List<long> Solve(Grid grid, Coordinate start, HashSet<int> cycle)
    {
        List<long> ticks = new (cycle.Count);
        Dictionary<Coordinate, ISet<Coordinate>> cache = [];

        HashSet<Coordinate> heads = [start];
        for (var i = 1; i <= cycle.Max(); i++)
        {
            var next = (from pos in heads
                        from n in GetNeighbours(grid, pos, cache)
                        select n).ToHashSet();

            if (cycle.Contains(i))
            {
                ticks.Add(next.Count);
            }

            (heads, next) = (next, heads);
        }

        return ticks;
    }

    private static IEnumerable<Coordinate> GetNeighbours(Grid grid, Coordinate c, IDictionary<Coordinate, ISet<Coordinate>> cache)
    {
        if (cache.TryGetValue(c, out var cached))
        {
            return cached;
        }
        var set = (from n in grid.Neighbours(c)
                   let x = new Coordinate(n.x % grid.Width, n.y % grid.Height)
                   where grid[x] == '.'
                   select n
                   )
            .ToHashSet();

        cache[c] = set;
        return set;
    }
}

readonly record struct Item(string name, int n);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>.*): (?<n>\d+)$")]
    public static partial Regex MyRegex();
}

public class AoC202321Tests
{
    private readonly ITestOutputHelper output;
    public AoC202321Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        var sut = new AoC202321(Read.Sample(1).Lines().ToArray(), new TestWriter(output));
        Assert.Equal(16, sut.Part1(6));
    }

    //[Theory]
    //[InlineData(6, 16)]
    //[InlineData(10,50)]
    //[InlineData(50,1594)]
    //[InlineData(100, 6536)]
    //[InlineData(500, 167004)]
    //[InlineData(1000, 668697)] 
    //[InlineData(5000, 16733044)]
    internal void TestPart2(int steps, int expected)
    {
        var sut = new AoC202321(Read.Sample(2).Lines().ToArray(), new TestWriter(output));
        Assert.Equal(expected, sut.Part2(steps));
    }
}

enum Direction { N, E, S, W }


class Grid
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.y;
    public int Width => endmarker.x;
    public Grid(string[] input, char empty = '.')
    {
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 where input[y][x] != empty
                 select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        endmarker = new(input[0].Length, input.Length);
        this.empty = empty;
    }

    internal Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }

    public Grid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new Grid(builder.ToImmutable(), empty, endmarker);
    }
    public Coordinate Find(char c) => items.Where(i => i.Value == c).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, endmarker.y)
        from x in Range(origin.x, endmarker.x)
        select new Coordinate(x, y);

    static (int, int)[] delta = [
        (-1, 0),
        (0, 1),
        (1, 0),
        (0, -1)
    ];

    public IEnumerable<Coordinate> Neighbours(Coordinate p) => from d in delta
                                                               let res = p + d
                                                               where IsValid(res)
                                                               select res;

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

