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

static class Ex
{
    public static IEnumerable<int> AsRange(this int[] ints) => Range(ints[0], ints[1] - ints[0]);

    public static bool HasAtLeastOneGroupOfAtLeast2AdjacentSameDigits(this int[] digits)
        => digits.GroupBy(i => i).Any(g => g.Count() >= 2);
    public static bool HasAtLeastOneGroupOfExactly2AdjacentSameDigits(this int[] digits)
        => digits.GroupBy(i => i).Any(g => g.Count() == 2);


    public static int[] ToDigits(this int n)
    {
        int[] digits = new int[6];
        int t = n;
        for (int i = 5; i >= 0; i--)
        {
            digits[i] = t % 10;
            t /= 10;
        }
        return digits;
    }

    public static bool IsAscending(this int[] digits)
        => digits.Take(digits.Length - 1).Zip(digits.Skip(1)).All(x => x.First <= x.Second);

}