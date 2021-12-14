namespace AdventOfCode.Year2016.Day06;

public class AoC201606
{
    public static string[] input = Read.InputLines(typeof(AoC201606));

    public object Part1() => new Accumulator().Decode(input, 8, false);
    public object Part2() => new Accumulator().Decode(input, 8, true);
}