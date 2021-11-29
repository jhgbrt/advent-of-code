using static AdventOfCode.Year2020.Day18.AoC;

using P1 = AdventOfCode.Year2020.Day18.Part1.Runner;
using P2 = AdventOfCode.Year2020.Day18.Part2.Runner;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day18
{
    partial class AoC
    {
        internal static Result Part1() => Run(() => P1.Run());
        internal static Result Part2() => Run(() => P2.Run());
    }
}