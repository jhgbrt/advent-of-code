using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static AoC;
using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;

var example = File.ReadLines("example.txt").AsBlocks();
var (e1, e2) = (Part1(example), Part2(example));
Debug.Assert((e1, e2) == (11, 6), $"{(e1, e2)}");

var input = File.ReadLines("input.txt").AsBlocks();
Console.WriteLine((Part1(input), Part2(input)));

static class AoC
{
    public static long Part1(Blocks blocks) => blocks.Select(block =>
        block.SelectMany(c => c).Distinct().Count()
    ).Sum();
    public static long Part2(Blocks blocks) => blocks.Select(block =>
        block.Aggregate(Enumerable.Range('a', 26).Select(i => (char)i).AsEnumerable(), (l, u) => l.Intersect(u)).Count()
    ).Sum();
    internal static Blocks AsBlocks(this IEnumerable<string> lines)
    {
        var enumerator = lines.GetEnumerator();
        while (enumerator.MoveNext())
            yield return GetBlock(enumerator);
    }
    private static IEnumerable<string> GetBlock(IEnumerator<string> enumerator)
    {
        while (!string.IsNullOrEmpty(enumerator.Current))
        {
            yield return enumerator.Current;
            if (!enumerator.MoveNext()) break;
        }
    }
}
