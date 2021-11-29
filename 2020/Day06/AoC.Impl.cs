
using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;

namespace AdventOfCode.Year2020.Day06;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input.AsBlocks()));
    internal static Result Part2() => Run(() => Part2(input.AsBlocks()));


    internal static long Part1(Blocks blocks) => blocks.Select(block =>
        block.SelectMany(c => c).Distinct().Count()
    ).Sum();
    internal static long Part2(Blocks blocks) => blocks.Select(block =>
        block.Aggregate(Enumerable.Range('a', 26).Select(i => (char)i).AsEnumerable(), (l, u) => l.Intersect(u)).Count()
    ).Sum();
}