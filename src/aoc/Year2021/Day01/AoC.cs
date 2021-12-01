using Microsoft.FSharp.Collections;

namespace AdventOfCode.Year2021.Day01;

public class AoC202101 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202101));
    static int[] numbers = input.Select(int.Parse).ToArray();
    public override object Part1() => numbers.Aggregate((previous: int.MaxValue, n: 0), (p, i) => (previous: i, n: p.n + (i > p.previous ? 1 : 0)));
    public override object Part2() => SeqModule.Windowed(3, numbers).Select(window => window.Sum()).Aggregate((previous: int.MaxValue, n: 0), (p, i) => (previous: i, n: p.n + (i > p.previous ? 1 : 0)));
}
