using static AdventOfCode.Year2017.Day09.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day09
{
    partial class AoC
    {
        internal static Result Part1() => Run(() => new GarbageProcessor().ProcessFile("input.txt").Score);
        internal static Result Part2() => Run(() => new GarbageProcessor().ProcessFile("input.txt").GarbageCount);
    }
}