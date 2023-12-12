namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD
{
    public AoCYYYYDD():this(Read.InputLines()) {}

    readonly ImmutableArray<Item> items;
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
    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines();
        var sut = new AoCYYYYDD(input);
        Assert.Equal(-1, sut.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines();
        var sut = new AoCYYYYDD(input);
        Assert.Equal(string.Empty, sut.Part1());
    }
}