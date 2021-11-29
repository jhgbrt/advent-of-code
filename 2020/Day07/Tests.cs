using static AdventOfCode.Year2020.Day07.AoC;

namespace AdventOfCode.Year2020.Day07;

public class Tests
{
    [Fact]
    public void Test1()
    {
        Assert.Equal(4, Part1("sample.txt"));
    }
    [Fact]
    public void Test2()
    {
        Assert.Equal(32, Part2("sample.txt"));
        Assert.Equal(126, Part2("sample2.txt"));
    }
}