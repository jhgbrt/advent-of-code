using static AdventOfCode.Year2019.Day03.AoC;
using static System.Math;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day03
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static int Part1(string[] input)
        {
            var points = input[0].Points().ToHashSet();
            var crossings = input[1].Points().Where(p => points.Contains(p));
            return crossings.Min(x => Abs(x.x) + Abs(x.y));
        }

        public static int Part2(string[] input)
        {
            var points1 = input[0].Points().Select((p, i) => (p, steps: i + 1)).ToLookup(x => x.p, x => x.steps);
            var points2 = input[1].Points().Select((p, i) => (p, steps: i + 1)).ToLookup(x => x.p, x => x.steps);
            var crossings = points2.Select(p => p.Key).Where(p => points1.Contains(p));

            var steps = crossings
                .Select(p => points1[p].Min() + points2[p].Min())
                .Min();

            return steps;
        }
    }
}
