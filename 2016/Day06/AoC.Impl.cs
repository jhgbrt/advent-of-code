namespace AdventOfCode.Year2016.Day06;

partial class AoC
{
    static bool test = false;
    static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    internal static Result Part1() => Run(() => new Accumulator().Decode(input, 8, false));
    internal static Result Part2() => Run(() => new Accumulator().Decode(input, 8, true));
}