
using static AdventOfCode.Year2015.Day12.AoC;

namespace AdventOfCode.Year2015.Day12;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(191164, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(87842, Part2().Value);
}