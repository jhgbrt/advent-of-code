using static AdventOfCode.Year2015.Day10.AoC;

namespace AdventOfCode.Year2015.Day10;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(252594, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(3579328, Part2().Value);
}