namespace AdventOfCode.Year2016.Day09;

partial class AoC
{
    public static string input = File.ReadLines("input.txt").First();

    internal static Result Part1() => Run(() => input.GetDecompressedSize(0));
    internal static Result Part2() => Run(() => input.GetDecompressedSize2(0, input.Length));

}