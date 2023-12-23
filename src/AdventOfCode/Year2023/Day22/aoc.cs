namespace AdventOfCode.Year2023.Day22;
public class AoC202322
{
    public AoC202322():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<Brick> items;
    internal IEnumerable<Brick> Items => items;
    public AoC202322(string[] input, TextWriter writer)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s).ToBrick())
            .OrderBy(b => b.Elevation)
            .ToImmutableArray();
        this.writer = writer;
    }

    public object Part1()
    {
        var set = new HashSet<Brick>();
        foreach (var item in items)
        {
            
        }
        return -1;
    }
    public object Part2() => "";
}

readonly record struct Item(int x1, int y1, int z1, int x2, int y2, int z2)
{
    public Brick ToBrick() => new(new(x1, y1, z1), new(x2, y2, z2));
}
readonly record struct Brick(Coordinate3D c1, Coordinate3D c2)
{
    public int Length => c1.ManhattanDistance(c2) + 1;
    public int Elevation => Min(c1.z, c2.z);
    public Coordinate3D Lowest => (c1, c2).MinBy(c => c.z);
    public Coordinate3D Highest => (c1, c2).MaxBy(c => c.z);
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<x1>\d+),(?<y1>\d+),(?<z1>\d+)~(?<x2>\d+),(?<y2>\d+),(?<z2>\d+)$")]
    public static partial Regex MyRegex();
}

public class AoC202322Tests
{
    private readonly AoC202322 sut;
    public AoC202322Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202322(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(7, sut.Items.Count());
    }

    [Fact]
    public void BrickLength()
    {
        var brick = new Brick(new(0, 0, 10), new(1, 0, 10));
        Assert.Equal(2, brick.Length);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(-1, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}