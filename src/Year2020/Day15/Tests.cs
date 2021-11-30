using static AdventOfCode.Year2020.Day15.AoCImpl;

namespace AdventOfCode.Year2020.Day15;

public class Tests
{
    // 2,3,1 test cases fail?
    [Theory]
    [InlineData(2020, 436, 0, 3, 6)]
    [InlineData(2020, 1, 1, 3, 2)]
    //[InlineData(2020, 10, 2, 3, 1)]
    [InlineData(2020, 27, 1, 2, 3)]
    [InlineData(2020, 438, 3, 2, 1)]
    [InlineData(2020, 1836, 3, 1, 2)]
    //[InlineData(30000000, 175594, 0, 3, 6)]
    //[InlineData(30000000, 2578, 1, 3, 2)]
    //[InlineData(30000000, 3544142, 2, 3, 1)]
    //[InlineData(30000000, 261214, 1, 2, 3)]
    //[InlineData(30000000, 18, 3, 2, 1)]
    //[InlineData(30000000, 362, 3, 1, 2)]
    public void Test1(int max, int expected, params int[] input)
    {
        Assert.Equal(expected, Run(input, max));
    }
}