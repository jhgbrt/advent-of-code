using static AdventOfCode.Year2015.Day17.AoC;

namespace AdventOfCode.Year2015.Day17;

public class Tests
{
    static int[] sample = new[] { 20, 15, 10, 5, 5 };
    [Fact]
    public void Test1() => Assert.Equal(1638, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(17, Part2().Value);
    [Fact]
    public void Test_Sample1() => Assert.Equal(4, Part1(sample, 25));
    [Fact]
    public void Test_Sample2() => Assert.Equal(3, Part2(sample, 25));
}