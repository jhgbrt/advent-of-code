using static AdventOfCode.Year2017.Day03.AoC;
using static AdventOfCode.Year2017.Day03.Spiral;

namespace AdventOfCode.Year2017.Day03;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(438, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(266330, Part2().Value);

    [Theory]
    [InlineData(1, 0, 0)]
    [InlineData(2, 1, 0)]
    [InlineData(3, 1, 1)]
    [InlineData(4, 0, 1)]
    [InlineData(5, -1, 1)]
    [InlineData(6, -1, 0)]
    [InlineData(7, -1, -1)]
    [InlineData(8, 0, -1)]
    [InlineData(9, 1, -1)]
    [InlineData(13, 2, 2)]
    [InlineData(16, -1, 2)]
    [InlineData(21, -2, -2)]
    public void ToEuclideanTests(int square, int expectedX, int expectedY)
    {
        var (x, y) = ToEuclidean(square);
        Assert.Equal((expectedX, expectedY), (x, y));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(12, 3)]
    [InlineData(23, 2)]
    [InlineData(1024, 31)]
    public void ShortestDistanceTest(int square, int expected)
    {
        var distance = DistanceToOrigin(square);
        Assert.Equal(expected, distance);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    [InlineData(3, 2)]
    [InlineData(4, 4)]
    [InlineData(5, 5)]
    [InlineData(6, 10)]
    [InlineData(7, 11)]
    [InlineData(8, 23)]
    public void SpiralValueTest(int square, int expected)
    {
        var value = SpiralValues().Take(square).Last().value;
        Assert.Equal(expected, value);
    }
}