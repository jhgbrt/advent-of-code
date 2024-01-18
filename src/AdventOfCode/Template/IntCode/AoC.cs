namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD
{
    public AoCYYYYDD() : this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    long[] program;
    IntCode intcode;

    public AoCYYYYDD(string[] input, TextWriter writer)
    {
        program = input.First().Split(',').Select(long.Parse).ToArray();
        intcode = new IntCode(program);
        this.writer = writer;
    }

    public int Part1()
    {
        return -1;
    }
    public int Part2() => -1;
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
    public AoCYYYYDDTests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoCYYYYDD(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(-1, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }
}