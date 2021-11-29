using static AdventOfCode.Year2017.Day04.AoC;

namespace AdventOfCode.Year2017.Day04;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(451, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(223, Part2().Value);
}