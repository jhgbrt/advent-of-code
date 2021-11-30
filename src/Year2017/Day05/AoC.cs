namespace AdventOfCode.Year2017.Day05;

public class AoCImpl : AoCBase
{
    public static int[] input = Read.InputLines(typeof(AoCImpl)).Select(int.Parse).ToArray();

    public override object Part1() => Jumps.CalculateJumps(input, v => 1);
    public override object Part2() => Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1);

}