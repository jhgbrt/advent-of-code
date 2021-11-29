namespace AdventOfCode.Year2017.Day19;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");
    internal static Result Part1() => Run(() => new MazeRunner(input).Traverse().code);
    internal static Result Part2() => Run(() => new MazeRunner(input).Traverse().steps);
}