using static AdventOfCode.Year2015.Day08.AoC;

namespace AdventOfCode.Year2015.Day08;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1333, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(2046, Part2().Value);
}