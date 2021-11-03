using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    static class AoC
    {
        public static int Part1(string[] input) => input.Select(int.Parse).Sum();

        public static int Part2(string[] input)
        {
            var ints = input.Select(int.Parse);
            var frequency = 0;
            var hashSet = new HashSet<int>();
            ints.EndlessRepeat().TakeWhile(i =>
            {
                hashSet.Add(frequency);
                frequency += i;
                return !hashSet.Contains(frequency);
            }).Last();
            return frequency;
        }

        static IEnumerable<int> EndlessRepeat(this IEnumerable<int> input)
        {
            while (true) foreach (var i in input) yield return i;
        }

    }
}
    