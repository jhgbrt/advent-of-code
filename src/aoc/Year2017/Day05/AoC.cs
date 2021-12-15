namespace AdventOfCode.Year2017.Day05;

public class AoC201705
{
    public static int[] input = Read.InputLines().Select(int.Parse).ToArray();

    public object Part1() => Jumps.CalculateJumps(input, v => 1);
    public object Part2() => Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1);

}