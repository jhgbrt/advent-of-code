namespace AdventOfCode.Year2017.Day14;

partial class AoC
{
    static string key = "hxtvlmkl";
    internal static Result Part1() => Run(() => Defrag.CountBitsInGrid(key));
    internal static Result Part2() => Run(() => Defrag.CountRegions(key));

}