using static AdventOfCode.Year2016.Day06.AoC201606;

namespace AdventOfCode.Year2016.Day06;

public class Tests
{
    static string[] input = Read.SampleLines();
    [Fact]
    public void Part1Test()
    {
        var result = new Accumulator().Decode(input, 6);
        Assert.Equal("easter", result);
    }

    [Fact]
    public void Part2Test()
    {
        var result = new Accumulator().Decode(input, 6, true);
        Assert.Equal("advent", result);
    }
}