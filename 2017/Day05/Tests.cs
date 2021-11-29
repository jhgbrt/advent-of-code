using static AdventOfCode.Year2017.Day05.AoC;

namespace AdventOfCode.Year2017.Day05;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(343467, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(24774780, Part2().Value);
}