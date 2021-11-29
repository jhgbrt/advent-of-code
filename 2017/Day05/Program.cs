using static AdventOfCode.Year2017.Day05.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day05
{
    partial class AoC
    {
        static bool test = false;
        public static int[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt").Select(int.Parse).ToArray();

        internal static Result Part1() => Run(() => Jumps.CalculateJumps(input, v => 1));
        internal static Result Part2() => Run(() => Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1));

    }
}