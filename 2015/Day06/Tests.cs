using static AdventOfCode.Year2015.Day06.AoC;

namespace AdventOfCode.Year2015.Day06;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(377891, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(14110788, Part2().Value);
}