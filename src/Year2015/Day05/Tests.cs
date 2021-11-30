using static AdventOfCode.Year2015.Day05.AoC201505;

namespace AdventOfCode.Year2015.Day05;

public class Tests
{

    [Theory]
    [InlineData("ugknbfddgicrmopn", true)]
    [InlineData("aaa", true)]
    [InlineData("jchzalrnumimnmhp", false)]
    [InlineData("haegwjzuvuyypxyu", false)]
    [InlineData("dvszwmarrgswjxmb", false)]
    public void IsNice1Tests(string input, bool expected) => Assert.Equal(expected, IsNice1(input));

    [Theory]
    [InlineData("qjhvhtzxzqqjkmpb", true)]
    [InlineData("xxyxx", true)]
    [InlineData("uurcxstgmygtbstg", false)]
    [InlineData("uurcxcstgmygtbstg", true)]
    [InlineData("ieodomkazucvgmuy", false)]
    public void IsNice2Tests(string input, bool expected) => Assert.Equal(expected, IsNice2(input));

}