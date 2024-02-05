namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD(string[] input, TextWriter writer)
{
    public AoCYYYYDD() : this(Read.InputLines(), Console.Out) {}
    readonly FiniteGrid grid = new FiniteGrid(input);
   

    public int Part1()
    {
        writer.WriteLine(grid);

        return -1;
    }
    public int Part2() => -1;
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