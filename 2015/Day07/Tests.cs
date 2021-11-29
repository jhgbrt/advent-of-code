using static AdventOfCode.Year2015.Day07.AoC;

namespace AdventOfCode.Year2015.Day07;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(46065, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(14134, Part2().Value);
}