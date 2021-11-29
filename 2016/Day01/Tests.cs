using static AdventOfCode.Year2016.Day01.AoC;

namespace AdventOfCode.Year2016.Day01;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(243, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(142, Part2().Value);

    [Theory]
    [InlineData(Bearing.N, Direction.R, Bearing.E)]
    [InlineData(Bearing.E, Direction.R, Bearing.S)]
    [InlineData(Bearing.S, Direction.R, Bearing.W)]
    [InlineData(Bearing.W, Direction.R, Bearing.N)]
    [InlineData(Bearing.N, Direction.L, Bearing.W)]
    [InlineData(Bearing.E, Direction.L, Bearing.N)]
    [InlineData(Bearing.S, Direction.L, Bearing.E)]
    [InlineData(Bearing.W, Direction.L, Bearing.S)]
    public void CompassTests(Bearing current, Direction turn, Bearing expected)
    {
        var compass = new Compass(current).Turn(turn);
        Assert.Equal(expected, compass.Bearing);
    }

    [Theory]
    [InlineData("R2, L3", 5)]
    [InlineData("R2, R2, R2", 2)]
    [InlineData("R5, L5, R5, R3", 12)]
    public void Test(string input, int expected)
    {
        var navigator = Navigate(input);
        var result = navigator.Part1;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("R1", -1)]
    [InlineData("R5, R5, R5, R5", 0)]
    [InlineData("R5, R4, R4, R4, L1", 1)]
    [InlineData("R8, R4, R4, R8", 4)]
    public void FirstPlaceVisitedTwiceTest(string input, int? expected)
    {
        Navigator navigator = Navigate(input);
        var result = navigator.Part2;
        Assert.Equal(expected, result);
    }
}