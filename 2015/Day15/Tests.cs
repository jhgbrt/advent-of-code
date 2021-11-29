using static AdventOfCode.Year2015.Day15.AoC;

namespace AdventOfCode.Year2015.Day15;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(21367368L, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1766400L, Part2().Value);
}