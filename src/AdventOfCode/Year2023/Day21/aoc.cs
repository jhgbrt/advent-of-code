namespace AdventOfCode.Year2023.Day21;
public class AoC202321
{
    public AoC202321():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    private readonly ImmutableHashSet<Coordinate> set;
    private readonly Coordinate start;
    private readonly int width;
    private readonly int height;
    public AoC202321(string[] input, TextWriter writer)
    {
        var all = from y in Range(0, input.Length)
                  from x in Range(0, input[0].Length)
                  select (x, y);

        set = (
            from c in all
            where input[c.y][c.x] != '#'
            select new Coordinate(c.x, c.y)
        ).ToImmutableHashSet();

        start = (
            from c in all
            where input[c.y][c.x] == 'S'
            select new Coordinate(c.x, c.y)
        ).Single();

        width = input[0].Length;
        height = input.Length;

        this.writer = writer;
    }

    public long Part1() => Part1(64);
    internal long Part1(int steps) => Solve(start, [steps]).First();
    public long Part2() => Part2(26501365);
    public long Part2(int n) 
    {
        var k = Range(0, 3);
        var x = Solve(start, k.Select(i => i*width+65).ToArray()).ToList();

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
   

    internal IEnumerable<long> Solve(Coordinate start, int[] steps)
    {
        Dictionary<Coordinate, ISet<Coordinate>> cache = [];
        HashSet<Coordinate> heads = [start];
        for (var i = 1; i <= steps.Max(); i++)
        {
            var next = (from pos in heads
                        from n in GetNeighbours(pos, cache)
                        select n).ToHashSet();

            if (steps.Contains(i))
            {
                yield return next.Count;
            }

            (heads, next) = (next, heads);
        }
    }
    private IEnumerable<Coordinate> GetNeighbours(Coordinate c, IDictionary<Coordinate, ISet<Coordinate>> cache)
    {
        if (cache.TryGetValue(c, out var cached))
        {
            return cached;
        }
        var neighbours = (from d in dirs
                   let n = c + d
                   let x = new Coordinate(Mod(n.x, width), Mod(n.y, height))
                   where set.Contains(x)
                   select n
                   )
            .ToHashSet();

        cache[c] = neighbours;
        return neighbours;
    }
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
   
}

public class AoC202321Tests
{
    private readonly ITestOutputHelper output;
    public AoC202321Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestPart1()
    {
        var sut = new AoC202321(Read.Sample(1).Lines().ToArray(), new TestWriter(output));
        Assert.Equal(16, sut.Part1(6));
    }

    [Theory]
    [InlineData(1000, 668697)]
    internal void TestPart2(int steps, int expected)
    {
        var sut = new AoC202321(Read.Sample(2).Lines().ToArray(), new TestWriter(output));
        Assert.Equal(expected, sut.Part2(steps));
    }
}