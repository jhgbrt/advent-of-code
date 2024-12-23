using Z3.Linq;

namespace AdventOfCode.Year2024.Day13;

public class AoC202413(Stream input)
{
    public AoC202413() : this(Read.InputStream()) {}
    readonly ImmutableArray<Machine> items = ReadInput(input).ToImmutableArray();

    private static IEnumerable<Machine> ReadInput(Stream stream)
    {
        using var sr = new StreamReader(stream);
        while (!sr.EndOfStream)
        {
            var buttonA = Regexes.Button().As<Delta>(sr.ReadLine()!);
            var buttonB = Regexes.Button().As<Delta>(sr.ReadLine()!);
            var prize = Regexes.Prize().As<Coordinate>(sr.ReadLine()!);
            yield return new Machine(buttonA, buttonB, prize);
            sr.ReadLine();
        }
    }

    public long Part1() => items.Select(m => m.Solve(Delta.Zero)).Sum();
    public long Part2() => items.Select(m => m.Solve(Delta.Square(10_000_000_000_000))).Sum();

}

readonly record struct Coordinate(long x, long y)
{
    public static Coordinate operator +(Coordinate left, Delta d) => new(left.x + d.dx, left.y + d.dy);
}
readonly record struct Delta(long dx, long dy)
{
    public readonly static Delta Zero = default;
    public static Delta Square(long value) => new(value, value);
}

readonly record struct Machine(Delta A, Delta B, Coordinate Prize)
{
    public long Solve(Delta delta)
    {
        var ((dxa, dya), (dxb, dyb), (x, y)) = (A, B, Prize + delta);
        using var z3 = new Z3Context();
        var theorem = from _ in z3.NewTheorem<(long a, long b)>()
                      where _.a * dxa + _.b * dxb == x
                      where _.a * dya + _.b * dyb == y
                      orderby 3 * _.a + _.b descending
                      select _;
        (long a, long b) = theorem.Solve();
        return 3 * a + b;
    }
}

static partial class Regexes
{
    [GeneratedRegex(@"Button [AB]: X(?<dx>[+\-\d]+), Y(?<dy>[+\-\d]+)")]
    public static partial Regex Button();

    [GeneratedRegex(@"Prize: X=(?<x>[\d]+), Y=(?<y>[\d]+)")]
    public static partial Regex Prize();
}

public class AoC202413Tests
{
    private readonly AoC202413 sut;
    public AoC202413Tests()
    {
        var input = Read.SampleStream();
        sut = new AoC202413(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(480, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(875318608908, sut.Part2());
    }
}

