namespace AdventOfCode.Year2017.Day07;

partial class AoC
{
    internal static Result Part1() => Run(() => Tree.Parse(File.ReadAllText("input.txt")).Root.Label);
    internal static Result Part2() => Run(() => Tree.Parse(File.ReadAllText("input.txt")).FindInvalidNode().RebalancingWeight);
}