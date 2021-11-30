namespace AdventOfCode.Year2017.Day07;

public class AoC201707 : AoCBase
{
    public override object Part1() => Tree.Parse(Read.InputText(typeof(AoC201707))).Root.Label;
    public override object Part2() => Tree.Parse(Read.InputText(typeof(AoC201707))).FindInvalidNode().RebalancingWeight;
}