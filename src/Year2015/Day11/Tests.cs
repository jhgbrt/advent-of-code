using static AdventOfCode.Year2015.Day11.AoCImpl;

namespace AdventOfCode.Year2015.Day11;

public class Tests
{

    [Theory]
    [InlineData("abc", true)]
    [InlineData("i", false)]
    [InlineData("oasdf", false)]
    [InlineData("l", false)]
    public void DoesNotContainOILTest(string input, bool expected)
    {
        Assert.Equal(expected, DoesNotContainOIL(input.ToCharArray()));
    }

    [Theory]
    [InlineData("aabb", true)]
    [InlineData("aabbccdd", true)]
    [InlineData("abcdefg", false)]
    [InlineData("aaabcdef", false)]
    public void ContainsTwoPairsTest(string input, bool expected)
    {
        Assert.Equal(expected, ContainsTwoPairs(input.ToCharArray()));
    }
    [Theory]
    [InlineData("cqjxxyyz", false)]
    [InlineData("acbedgfh", false)]
    [InlineData("abcedgfh", true)]
    public void IncludesStraightOfThreeTest(string input, bool expected)
    {
        Assert.Equal(expected, IncludesStraightOfThree(input.ToCharArray()));
    }
}