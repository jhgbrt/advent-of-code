using static AdventOfCode.Year2015.Day16.AoC;

namespace AdventOfCode.Year2015.Day16;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(213, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(323, Part2().Value);
}