using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode
{
    static class AoC
    {
        public static string Part1(string[] input) => ToGrid(input).FindGridWithLowestHeight().ToString();

        public static int Part2(string[] input) => ToGrid(input).FindGridWithLowestHeight().Ticks;

        static Grid ToGrid(string[] input) => new Grid(from s in input select Point.Parse(s));

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
