namespace AdventOfCode.Year2017.Day21;

public class AoC201721
{
    static Rule[] rules = Read.InputLines().Select(Rule.Parse).ToArray();
    static char[,] input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
    public object Part1() => new ExpandingGrid(input).Expand(rules, 5).Count('#');
    public object Part2() => new ExpandingGrid(input).Expand(rules, 18).Count('#');
}