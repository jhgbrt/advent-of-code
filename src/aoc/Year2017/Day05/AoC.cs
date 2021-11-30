namespace AdventOfCode.Year2017.Day05;

public class AoC201705 : AoCBase
{
    public static int[] input = Read.InputLines(typeof(AoC201705)).Select(int.Parse).ToArray();

    public override object Part1() => Jumps.CalculateJumps(input, v => 1);
    public override object Part2() => Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1);

}