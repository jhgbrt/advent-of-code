namespace AdventOfCode.Year2023.Day23;
public class AoC202323
{
    public AoC202323():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly FiniteGrid grid;
    public AoC202323(string[] input, TextWriter writer)
    {
        grid = new FiniteGrid(input);
        this.writer = writer;
    }

    public object Part1()
    {
        return -1;
    }
    public object Part2() => "";
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