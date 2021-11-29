using static AdventOfCode.Year2016.Day03.AoC;

namespace AdventOfCode.Year2016.Day03;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1032, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1838, Part2().Value);

    [Theory]
    [InlineData(5, 10, 25, false)]
    [InlineData(5, 25, 10, false)]
    [InlineData(25, 5, 10, false)]
    [InlineData(25, 10, 5, false)]
    [InlineData(10, 25, 5, false)]
    [InlineData(10, 5, 25, false)]
    [InlineData(5, 10, 12, true)]
    [InlineData(5, 12, 10, true)]
    [InlineData(12, 5, 10, true)]
    [InlineData(12, 10, 5, true)]
    [InlineData(10, 12, 5, true)]
    [InlineData(10, 5, 12, true)]
    [InlineData(3, 4, 5, true)]
    public void Test(int x, int y, int z, bool expected)
    {
        Assert.Equal(new Triangle(x, y, z).IsValid, expected);
    }
}