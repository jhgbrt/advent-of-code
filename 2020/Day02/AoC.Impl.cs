namespace AdventOfCode.Year2020.Day02;

partial class AoC
{
    internal static Result Part1() => Run(() => Driver.Part1("input.txt"));
    internal static Result Part2() => Run(() => Driver.Part2("input.txt"));
}
record Entry(int Min, int Max, char Letter, string Password);
