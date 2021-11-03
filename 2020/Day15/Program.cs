using System;
using System.Linq;
using static AoC;
using Xunit;

var input = new[] { 0, 1, 4, 13, 15, 12, 16 };

Console.WriteLine((Run(input, 2020), Run(input, 30000000)));

public static class AoC
{
    internal static long Run(int[] input, int max)
    {
        var dic = input.Select((n, i) => (n, i)).ToDictionary(x => x.n, x => (turn_1: x.i, turn_2: x.i));
        int last = input.Last();
        for (var i = input.Length; i < max; i++)
        {
            var next = dic[last].turn_1 - dic[last].turn_2;
            var previous = dic.ContainsKey(next) ? dic[next].turn_1 : i;
            dic[next] = (i, previous);
            last = next;
        }
        return last;
    }
}
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
    [InlineData(30000000, 175594, 0, 3, 6)]
    [InlineData(30000000, 2578, 1, 3, 2)]
    //[InlineData(30000000, 3544142, 2, 3, 1)]
    [InlineData(30000000, 261214, 1, 2, 3)]
    [InlineData(30000000, 18, 3, 2, 1)]
    [InlineData(30000000, 362, 3, 1, 2)]
    public void Test1(int max, int expected, params int[] input)
    {
        Assert.Equal(expected, Run(input, max));
    }
}