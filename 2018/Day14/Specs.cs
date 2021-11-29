namespace AdventOfCode.Year2018.Day14;

public class Specs
{
    string[] input = new[] { "" };

    [Fact]
    public void GetDigits()
    {
        Assert.Equal(new[] { 0 }, 0.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1 }, 1.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 8 }, 8.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 9 }, 9.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 0 }, 10.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 1 }, 11.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 2 }, 12.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 3 }, 13.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 4 }, 14.GetDigits().Reverse().ToArray());
        Assert.Equal(new[] { 1, 9 }, 19.GetDigits().Reverse().ToArray());
    }

    [Theory]
    [InlineData(9, 5158916779)]
    [InlineData(5, 0124515891)]
    [InlineData(18, 9251071085)]
    [InlineData(2018, 5941429882)]
    public void TestPart1(int n, long expected)
    {
        var result = AoC.Part1(n);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(9, 51589)]
    [InlineData(5, 01245)]
    [InlineData(18, 92510)]
    [InlineData(2018, 59414)]
    //[InlineData(2018, 327901)]
    public void TestPart2(int expected, int input)
    {
        var result = AoC.Part2(input);
        Assert.Equal(expected, result);
    }
}
