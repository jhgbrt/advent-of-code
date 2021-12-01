namespace AdventOfCode.Year2020.Day09;

public class AoC202009 : AoCBase
{
    static long[] input = Read.InputLines(typeof(AoC202009)).Select(long.Parse).ToArray();

    public override object Part1() => input.InvalidNumbers(25).First();
    public override object Part2() => input.FindEncryptionWeakness((long)Part1());

}