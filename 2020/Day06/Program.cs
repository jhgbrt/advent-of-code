using static AdventOfCode.Year2020.Day06.AoC;

using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;


Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day06
{
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
}

static class Ex
{
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

public class Tests
{
    [Fact]
    public void TestExample()
    {
        var example = File.ReadLines("sample.txt").AsBlocks();
        var (e1, e2) = (Part1(example), Part2(example));
        Debug.Assert((e1, e2) == (11, 6), $"{(e1, e2)}");

    }
}