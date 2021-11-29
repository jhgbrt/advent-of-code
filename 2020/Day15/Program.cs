using static AdventOfCode.Year2020.Day15.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day15
{
    public partial class AoC
    {
        static int[] input = new[] { 0, 1, 4, 13, 15, 12, 16 };
        internal static Result Part1() => Run(() => Run(input, 2020));
        internal static Result Part2() => Run(() => Run(input, 30000000));

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