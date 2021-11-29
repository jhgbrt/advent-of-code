
using static AdventOfCode.Year2017.Day06.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day06
{
    partial class AoC
    {
        static bool test = false;
        public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).steps);
        internal static Result Part2() => Run(() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).loopSize);

    }
}


