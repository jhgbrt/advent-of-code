using static AdventOfCode.Year2015.Day13.AoC;

namespace AdventOfCode.Year2015.Day13;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(618, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(601, Part2().Value);
}