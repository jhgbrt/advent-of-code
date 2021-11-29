using static AdventOfCode.Year2015.Day23.AoC;

namespace AdventOfCode.Year2015.Day23;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(255, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(334, Part2().Value);
}