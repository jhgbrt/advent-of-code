namespace AdventOfCode.Year2016.Day06;

public class AoCImpl : AoCBase
{
    public static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => new Accumulator().Decode(input, 8, false);
    public override object Part2() => new Accumulator().Decode(input, 8, true);
}