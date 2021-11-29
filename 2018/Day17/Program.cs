using static AdventOfCode.Year2018.Day17.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day17
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static int Part1(string[] input)
        {
            var grid = Grid.Parse(input);
            grid.Simulate();
            return grid.NofWaterReachableTiles;
        }

        public static int Part2(string[] input)
        {
            var grid = Grid.Parse(input);
            grid.Simulate();
            return grid.NofWaterTiles;
        }

    }
}
