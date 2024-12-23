namespace AdventOfCode.Year2024.Day22;

public class AoC202422(string[] input)
{
    public AoC202422() : this(Read.InputLines()) {}

    long[] numbers = input.Select(long.Parse).ToArray();

    public long Part1() => numbers.Select(Repeat).Sum();

    private long Repeat(long secret)
    {
        for (int i = 0; i < 2000; i++)
        {
            secret = ((secret << 6) ^ secret) & (0x1000000-1);
            secret = ((secret >> 5) ^ secret) & (0x1000000-1);
            secret = ((secret << 11) ^ secret) & (0x1000000-1);
        }
        return secret;
    }

    public int Part2() => -1;
}

public class AoC202422Tests(ITestOutputHelper output)
{

    [Fact]
    public void TestParsing()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202422(input);
    }

    [Fact]
    public void Test()
    {
        output.WriteLine(Math.Pow(2, 24).ToString());
        output.WriteLine(0x1000000.ToString());

        Assert.Equal(0x1000000, (int)Math.Pow(2, 24));
        Assert.Equal(0x0800000, (int)Math.Pow(2, 23));
        output.WriteLine($"{(int)Math.Pow(2, 23)}");
        Assert.Equal(123123887681726 % 0x1000000, 123123887681726 & (0x1000000 - 1));
    }

    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202422(input);
        Assert.Equal(37327623, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines(2);
        var sut = new AoC202422(input);
        Assert.Equal(-1, sut.Part2());
    }
}