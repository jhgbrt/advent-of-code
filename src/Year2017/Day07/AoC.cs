namespace AdventOfCode.Year2017.Day07;

public class AoCImpl : AoCBase
{
    public override object Part1() => Tree.Parse(Read.InputText(typeof(AoCImpl))).Root.Label;
    public override object Part2() => Tree.Parse(Read.InputText(typeof(AoCImpl))).FindInvalidNode().RebalancingWeight;
}