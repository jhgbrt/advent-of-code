using static AdventOfCode.Year2015.Day22.AoC;

namespace AdventOfCode.Year2015.Day22;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1269, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1309, Part2().Value);
}