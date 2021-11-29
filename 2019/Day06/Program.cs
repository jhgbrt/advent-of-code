
using static AdventOfCode.Year2019.Day06.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day06
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static int Part1(string[] input)
        {
            var graph = input.CreateGraph();
            return graph.Vertices
                .Select(v => graph.CountDistance("COM", v))
                .Sum();
        }

        public static int Part2(string[] input)
            => input.CreateGraph().CountDistance("YOU", "SAN") - 2;
    }
}
