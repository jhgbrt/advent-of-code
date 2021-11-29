
using static AdventOfCode.Year2019.Day05.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day05
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input).Last());
        internal static Result Part2() => Run(() => string.Join(",", Part2(input)));
        public static IEnumerable<int> Part1(string[] program) => IntCode.Run(Parse(program), 1);
        public static IEnumerable<int> Part1(string[] program, int input) => IntCode.Run(Parse(program), input);
        public static IEnumerable<int> Part2(string[] program) => IntCode.Run(Parse(program), 5);

        static ImmutableArray<int> Parse(string[] input) => input[0].Split(',').Select(int.Parse).ToImmutableArray();


    }
}