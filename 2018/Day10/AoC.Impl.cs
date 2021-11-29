namespace AdventOfCode.Year2018.Day10;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));

    public static string Part1(string[] input) => ToGrid(input).FindGridWithLowestHeight().Decode();

    public static int Part2(string[] input) => ToGrid(input).FindGridWithLowestHeight().Ticks;

    internal static Grid ToGrid(string[] input) => new Grid(from s in input select Point.Parse(s));

}