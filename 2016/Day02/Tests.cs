using static AdventOfCode.Year2016.Day02.AoC;

namespace AdventOfCode.Year2016.Day02;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal("24862", Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal("46C91", Part2().Value);

    static string[] testinputs = new[] { "ULL", "RRDDD", "LURDL", "UUUUD" };

    [Fact]
    public void ExamplesPart1()
    {
        var code = GetCode(testinputs, keypad1);
        Assert.Equal("1985", code);
    }


    [Fact]
    public void ExamplesPart2()
    {
        var code = GetCode(testinputs, keypad2);
        Assert.Equal("5DB3", code);
    }

}