namespace AdventOfCode.Year2016.Day06;

public class AoC201606 : AoCBase
{
    public static string[] input = Read.InputLines(typeof(AoC201606));

    public override object Part1() => new Accumulator().Decode(input, 8, false);
    public override object Part2() => new Accumulator().Decode(input, 8, true);
}