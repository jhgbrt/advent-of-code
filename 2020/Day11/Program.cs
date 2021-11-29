using static AdventOfCode.Year2020.Day11.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day11
{
    partial class AoC
    {
        static Grid grid = Grid.FromFile("input.txt");

        internal static Result Part1() => Run(() => grid.Handle(Grid.Rule1));
        internal static Result Part2() => Run(() => grid.Handle(Grid.Rule2));

    }
}
