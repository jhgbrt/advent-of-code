namespace AdventOfCode.Year2020.Day09;

public class AoC202009
{
    static long[] input = Read.InputLines().Select(long.Parse).ToArray();

    public object Part1() => input.InvalidNumbers(25).First();
    public object Part2() => input.FindEncryptionWeakness((long)Part1());

}