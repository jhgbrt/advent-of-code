using static AdventOfCode.Year2016.Day02.AoC201602;

namespace AdventOfCode.Year2016.Day02;

public class Tests
{

    static string[] testinputs = new[] { "ULL", "RRDDD", "LURDL", "UUUUD" };

    [Fact]
    public void ExamplesPart1()
    {
        var code = GetCode(testinputs, Keypad1);
        Assert.Equal("1985", code);
    }


    [Fact]
    public void ExamplesPart2()
    {
        var code = GetCode(testinputs, Keypad2);
        Assert.Equal("5DB3", code);
    }

}