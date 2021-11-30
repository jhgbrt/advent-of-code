using static AdventOfCode.Year2015.Day20.AoC201520;

namespace AdventOfCode.Year2015.Day20;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(786240, Part1Impl());
    [Fact]
    public void Test2() => Assert.Equal(831600, Part2Impl());
}