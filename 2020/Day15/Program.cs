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