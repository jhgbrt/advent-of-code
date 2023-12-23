namespace AdventOfCode.Year2023.Day21;
public class AoC202321
{
    public AoC202321():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    public AoC202321(string[] input, TextWriter writer)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public object Part1()
    {
        foreach (var item in items)
            Console.WriteLine(item);

        return -1;
    }
    public object Part2() => "";
}

readonly record struct Item(string name, int n);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>.*): (?<n>\d+)$")]
    public static partial Regex MyRegex();
}

public class AoC202321Tests
{
    private readonly AoC202321 sut;
    public AoC202321Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202321(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(2, sut.Items.Count());
        Assert.Equal("foo", sut.Items.First().name);
        Assert.Equal(1, sut.Items.First().n);
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