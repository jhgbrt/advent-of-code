using Sprache;

namespace AdventOfCode.Year2023.Day21;
public class AoC202321
{
    public AoC202321():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly string[] input;
    private ImmutableHashSet<Coordinate> set;
    private Coordinate start;
    private int width;
    private int height;
    public AoC202321(string[] input, TextWriter writer)
    {
        this.input = input;
        set = (
            from y in Range(0, input.Length)
            from x in Range(0, input[0].Length)
            where input[y][x] != '#'
            select new Coordinate(x, y)
        ).ToImmutableHashSet();

        start = (from y in Range(0, input.Length)
                 from x in Range(0, input[0].Length)
                 where input[y][x] == 'S'
                 select new Coordinate(x, y)
                ).Single();
        width = set.Max(c => c.x) + 1;
        height = set.Max(c => c.y) + 1;
        this.writer = writer;
    }

    public long Part1() => Part1(64);
    internal long Part1(int steps) => Solve(start, [steps]).First();
    public long Part2() => Part2(26501365);
    public long Part2(int n) 
    {
        var k = Range(0, 3);
        var x = Solve(start, k.Select(i => i*width+65).ToHashSet()).ToList();

        var q = (from i in k
                 select (x: (decimal)i * width + 65, y: (decimal)x[i])
            ).ToArray();

        var y01 = (q[1].y - q[0].y) / (q[1].x - q[0].x);
        var y12 = (q[2].y - q[1].y) / (q[2].x - q[1].x);
        var y012 = (y12 - y01) / (q[2].x - q[0].x);

        return (long)(q[0].y + y01 * (n - q[0].x) + y012 * (n - q[0].x) * (n - q[1].x));
        
    }

    static readonly (int x, int y)[] dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)]; 
    static int Mod(int n, int m) => ((n % m) + m) % m;
   

    internal IEnumerable<long> Solve(Coordinate start, HashSet<int> cycle)
    {
        List<long> ticks = new(cycle.Count);
        Dictionary<Coordinate, ISet<Coordinate>> cache = [];

        HashSet<Coordinate> heads = [start];
        for (var i = 1; i <= cycle.Max(); i++)
        {
            var next = (from pos in heads
                        from n in GetNeighbours(set, pos, cache)
                        select n).ToHashSet();

            if (cycle.Contains(i))
            {
                yield return next.Count;
            }

            (heads, next) = (next, heads);
        }
    }
    private IEnumerable<Coordinate> GetNeighbours(ImmutableHashSet<Coordinate> set, Coordinate c, IDictionary<Coordinate, ISet<Coordinate>> cache)
    {
        if (cache.TryGetValue(c, out var cached))
        {
            return cached;
        }
        var neighbours = (from d in dirs
                   let n = c + d
                   let x = new Coordinate(n.x % width, n.y % height)
                   where set.Contains(new(Mod(n.x, width), Mod(n.y, height)))
                   select n
                   )
            .ToHashSet();

        cache[c] = neighbours;
        return neighbours;
    }
}

readonly record struct Item(string name, int n);
readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
   
}

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

    [Theory]
    [InlineData(6, 16)]
    [InlineData(10, 50)]
    [InlineData(50, 1594)]
    [InlineData(100, 6536)]
    [InlineData(500, 167004)]
    [InlineData(1000, 668697)]
    [InlineData(5000, 16733044)]
    internal void TestPart2(int steps, int expected)
    {
        var sut = new AoC202321(Read.Sample(2).Lines().ToArray(), new TestWriter(output));
        Assert.Equal(expected, sut.Part2(steps));
    }
}
