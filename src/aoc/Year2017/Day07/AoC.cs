namespace AdventOfCode.Year2017.Day07;

public class AoC201707
{
    public object Part1() => Tree.Parse(Read.InputText()).Root.Label;
    public object Part2() => Tree.Parse(Read.InputText()).FindInvalidNode().RebalancingWeight;
}