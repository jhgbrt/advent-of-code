using static AdventOfCode.Year2017.Day01.AoC;

namespace AdventOfCode.Year2017.Day01;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1034, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1356, Part2().Value);

    [Theory]
    [InlineData("", 0)]
    [InlineData("1122", 3)]
    [InlineData("1111", 4)]
    [InlineData("1234", 0)]
    [InlineData("91212129", 9)]
    public void TestsPart1(string input, int expected)
    {
        Assert.Equal(expected, Captcha.Calculate(input, 1));
    }
    [Theory]
    [InlineData("1212", 6)]
    [InlineData("1221", 0)]
    [InlineData("123425", 4)]
    [InlineData("123123", 12)]
    [InlineData("12131415", 4)]
    public void TestsPart2(string input, int expected)
    {
        Assert.Equal(expected, Captcha.Calculate(input, input.Length / 2));
    }
}