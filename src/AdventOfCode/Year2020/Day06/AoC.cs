
using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;

namespace AdventOfCode.Year2020.Day06;

public class AoC202006
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input.AsBlocks());
    public object Part2() => Part2(input.AsBlocks());


    internal static long Part1(Blocks blocks) => blocks.Select(block =>
        block.SelectMany(c => c).Distinct().Count()
    ).Sum();
    internal static long Part2(Blocks blocks) => blocks.Select(block =>
        block.Aggregate(Enumerable.Range('a', 26).Select(i => (char)i).AsEnumerable(), (l, u) => l.Intersect(u)).Count()
    ).Sum();
}