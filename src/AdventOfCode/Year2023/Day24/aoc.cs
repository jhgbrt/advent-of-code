using Z3.Linq;
namespace AdventOfCode.Year2023.Day24;
public class AoC202324
{
    public AoC202324() : this(Read.InputLines(), Console.Out) { }
    readonly TextWriter writer;
    readonly ImmutableArray<Vector> vectors;
    internal IEnumerable<Vector> Items => vectors;

    public AoC202324(string[] input, TextWriter writer)
    {
        vectors = input.Select(s => Regexes.MyRegex().As<Item>(s)).Select(item => item.ToVector()).ToImmutableArray();
        this.writer = writer;
    }
    public int Part1() => Part1(200000000000000, 400000000000000);

    public int Part1(long min, long max) => Solve(vectors, min, max);
    public long Part2() => SolvePart2(vectors);

    private static int Solve(IReadOnlyList<Vector> vectors, long min, long max)
    => (from i in Range(0, vectors.Count)
        from j in Range(i + 1, vectors.Count - i - 1)
        let v1 = vectors[i]
        let v2 = vectors[j]
        where v1.Slope != v2.Slope
        let x = (v2.Intercept - v1.Intercept) / (v1.Slope - v2.Slope)
        let y = v1.Slope * x + v1.Intercept
        where min <= x && x <= max && min <= y && y <= max
        let t1 = (x - v1.position.x) / v1.velocity.dx
        let t2 = (x - v2.position.x) / v2.velocity.dx
        where t1 >= 0 && t2 >= 0
        select (v1, v2)).Count();

    static long SolvePart2(ImmutableArray<Vector> vectors)
    {
        var (
            ((x1, y1, z1), (dx1, dy1, dz1)),
            ((x2, y2, z2), (dx2, dy2, dz2)),
            ((x3, y3, z3), (dx3, dy3, dz3))
            ) = vectors[0..3].ToTuple3();

        using (var ctx = new Z3Context())
        {

            var theorem = from _ in ctx.NewTheorem<(long x, long y, long z, long vx, long vy, long vz, long t0, long t1, long t2)>()
                          where _.x + _.vx * _.t0 == x1 + _.t0 * dx1
                          where _.y + _.vy * _.t0 == y1 + _.t0 * dy1
                          where _.z + _.vz * _.t0 == z1 + _.t0 * dz1
                          where _.x + _.vx * _.t1 == x2 + _.t1 * dx2
                          where _.y + _.vy * _.t1 == y2 + _.t1 * dy2
                          where _.z + _.vz * _.t1 == z2 + _.t1 * dz2
                          where _.x + _.vx * _.t2 == x3 + _.t2 * dx3
                          where _.y + _.vy * _.t2 == y3 + _.t2 * dy3
                          where _.z + _.vz * _.t2 == z3 + _.t2 * dz3
                          select _;
            var (x, y, z, _, _, _, _, _, _) = theorem.Solve();

            return x + y + z;
        }
    }

}

readonly record struct Item(long x, long y, long z, int dx, int dy, int dz)
{
    public Vector ToVector() => new(new(x, y, z), new(dx, dy, dz));
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<x>[^,]+), (?<y>[^,]+), (?<z>[^,]+) @ (?<dx>[^,]+), (?<dy>[^,]+), (?<dz>.+)$")]
    public static partial Regex MyRegex();
}

public class AoC202324Tests
{
    private readonly AoC202324 sut;
    public AoC202324Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202324(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(5, sut.Items.Count());
        // 19, 13, 30 @ -2,  1, -2
        var item = sut.Items.First();
        Assert.Equal(new Vector(new(19, 13, 30), new(-2, 1, -2)), item);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(2, sut.Part1(7,27));
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(47, sut.Part2());
    }
}

readonly record struct Vector(Coordinate position, Velocity velocity)
{
    public double Slope => (double)velocity.dy / velocity.dx;
    public double Intercept => position.y - Slope * position.x;
}
readonly record struct Velocity(long dx, long dy, long dz);
readonly record struct Coordinate(long x, long y, long z)
{
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate left, (int dx, int dy, int dz) p) => new(left.x + p.dx, left.y + p.dy, left.z + p.dz);
}

