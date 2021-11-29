using static AdventOfCode.Year2015.Day19.AoC;

namespace AdventOfCode.Year2015.Day19;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(535, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(212, Part2().Value);
}