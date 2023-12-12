namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD
{
    public AoCYYYYDD():this(Read.InputLines()) {}

    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    public AoCYYYYDD(string[] input)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableArray();
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

public class AoCYYYYDDTests
{
    private readonly AoCYYYYDD sut;
    public AoCYYYYDDTests()
    {
        var input = Read.SampleLines();
        sut = new AoCYYYYDD(input);
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