using static AdventOfCode.Year2016.Day06.AoC;

namespace AdventOfCode.Year2016.Day06;

public class Tests
{
    static string[] input = File.ReadAllLines("sample.txt");
    [Fact]
    public void Test1() => Assert.Equal("kjxfwkdh", Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal("xrwcsnps", Part2().Value);
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