using static AdventOfCode.Year2015.Day14.AoC;

namespace AdventOfCode.Year2015.Day14;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(2660, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1256, Part2().Value);
}