using static AdventOfCode.Year2018.Day07.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day07
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));

        public static string Part1(string[] input) => new string(input.ToGraph().FindStepOrder().ToArray());
        public static int Part2(string[] input) => input.ToGraph().FindTotalDuration(5, 60);
    }
}
