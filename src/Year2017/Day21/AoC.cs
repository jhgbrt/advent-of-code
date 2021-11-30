namespace AdventOfCode.Year2017.Day21;

public class AoCImpl : AoCBase
{
    static Rule[] rules = Read.InputLines(typeof(AoCImpl)).Select(Rule.Parse).ToArray();
    static char[,] input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
    public override object Part1() => new ExpandingGrid(input).Expand(rules, 5).Count('#');
    public override object Part2() => new ExpandingGrid(input).Expand(rules, 18).Count('#');
}