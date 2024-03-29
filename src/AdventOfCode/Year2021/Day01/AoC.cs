namespace AdventOfCode.Year2021.Day01;

public class AoC202101
{
    static string[] input = Read.InputLines();
    static int[] numbers = input.Select(int.Parse).ToArray();
    public object Part1() => numbers.Aggregate(
        (prev: int.MaxValue, count: 0),
        (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))
        ).count;
    public object Part2() => numbers.Windowed(3).Select(window => window.Sum()).Aggregate(
        (prev: int.MaxValue, count: 0),
        (p, i) => (prev: i, count: p.count + (i > p.prev ? 1 : 0))
        ).count;
}
