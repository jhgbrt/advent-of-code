namespace AdventOfCode.Year2017.Day21;

partial class AoC
{
    static Rule[] rules = File.ReadLines("input.txt").Select(Rule.Parse).ToArray();
    static char[,] input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
    internal static Result Part1() => Run(() => new ExpandingGrid(input).Expand(rules, 5).Count('#'));
    internal static Result Part2() => Run(() => new ExpandingGrid(input).Expand(rules, 18).Count('#'));
}