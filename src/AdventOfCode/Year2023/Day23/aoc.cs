namespace AdventOfCode.Year2023.Day23;
public class AoC202323
{
    public AoC202323():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    public AoC202323(string[] input, TextWriter writer)
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

public class AoC202323Tests
{
    private readonly AoC202323 sut;
    public AoC202323Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202323(input, new TestWriter(output));
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