using static AdventOfCode.Year2015.Day09.AoC;

namespace AdventOfCode.Year2015.Day09;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(251, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(898, Part2().Value);
}