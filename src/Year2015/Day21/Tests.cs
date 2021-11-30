using static AdventOfCode.Year2015.Day21.AoC201521;

namespace AdventOfCode.Year2015.Day21;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(78, Part1Impl());
    [Fact]
    public void Test2() => Assert.Equal(148, Part2Impl());
}