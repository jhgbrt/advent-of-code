using static AdventOfCode.Year2018.Day10.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day10
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));

        public static string Part1(string[] input) => ToGrid(input).FindGridWithLowestHeight().Decode();

        public static int Part2(string[] input) => ToGrid(input).FindGridWithLowestHeight().Ticks;

        internal static Grid ToGrid(string[] input) => new Grid(from s in input select Point.Parse(s));

    }
    static class Ex
    {
        public static Grid FindGridWithLowestHeight(this Grid grid) => grid.KeepMoving().Where(g => g.Height < g.Move(1).Height).First();
        static IEnumerable<Grid> KeepMoving(this Grid grid)
        {
            while (true)
            {
                grid = grid.Move(1);
                yield return grid;
            }
        }
    }
}