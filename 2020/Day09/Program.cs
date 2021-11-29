using static AdventOfCode.Year2020.Day09.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day09
{
    partial class AoC
    {
        static long[] input = File.ReadLines("input.txt").Select(long.Parse).ToArray();

        internal static Result Part1() => Run(() => input.InvalidNumbers(25).First());
        internal static Result Part2() => Run(() => input.FindEncryptionWeakness((long)Part1().Value));

    }
}
