namespace AdventOfCode.Year2018.Day10;

public class AoC201810 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201810));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);

    public static string Part1(string[] input) => ToGrid(input).FindGridWithLowestHeight().Decode();

    public static int Part2(string[] input) => ToGrid(input).FindGridWithLowestHeight().Ticks;

    internal static Grid ToGrid(string[] input) => new Grid(from s in input select Point.Parse(s));

}