namespace AdventOfCode.Year2021.Day01;

public class AoC202101 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202101));
    static int[] numbers = input.Select(int.Parse).ToArray();
    public override object Part1() => numbers.Aggregate(
        (prev: int.MaxValue, count: 0), 
        (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))
        ).count;
    public override object Part2() => SeqModule.Windowed(3, numbers).Select(window => window.Sum()).Aggregate(
        (prev: int.MaxValue, count: 0),
        (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))
        ).count;
}
