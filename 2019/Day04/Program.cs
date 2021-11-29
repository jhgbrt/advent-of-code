using static AdventOfCode.Year2019.Day04.AoC;
using static System.Linq.Enumerable;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day04
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static int Part1(string[] input)
            => (
            from i in ParseInput(input)
            let d = i.ToDigits()
            where d.IsAscending() && d.HasAtLeastOneGroupOfAtLeast2AdjacentSameDigits()
            select d
            ).Count();

        public static int Part2(string[] input)
            => (
            from i in ParseInput(input)
            let d = i.ToDigits()
            where d.IsAscending() && d.HasAtLeastOneGroupOfExactly2AdjacentSameDigits()
            select d
            ).Count();

        static IEnumerable<int> ParseInput(string[] input)
            => input[0]
            .Split('-')
            .Select(int.Parse)
            .ToArray()
            .AsRange();
    }
}
