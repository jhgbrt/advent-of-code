namespace AdventOfCode.Year2018.Day20;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));
    public static int Part1(string[] input) => input.Single().Distances().Max();

    public static int Part2(string[] input) => input.Single().Distances().Where(i => i >= 1000).Count();
}