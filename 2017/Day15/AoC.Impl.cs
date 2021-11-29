namespace AdventOfCode.Year2017.Day15;

partial class AoC
{
    static int seedA = 722;
    static int seedB = 354;
    internal static Result Part1() => Run(() => Generator.GetNofMatches(seedA, seedB, 40_000_000));
    internal static Result Part2() => Run(() => Generator.GetNofMatches(seedA, seedB, 5_000_000, 4, 8));

}