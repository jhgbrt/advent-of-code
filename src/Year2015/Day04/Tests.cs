
using static AdventOfCode.Year2015.Day04.AoC201504;

namespace AdventOfCode.Year2015.Day04;

public class Tests
{
    [Theory]
    [InlineData("abcdef", 609043)]
    [InlineData("pqrstuv", 1048970)]
    public void Test(string input, int expected)
    {
        var result = Solve(input, 5);
        Assert.Equal(expected, result);
    }

}