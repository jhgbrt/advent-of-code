namespace AdventOfCode.Year2017.Day11;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));
    public override object Part1() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).distance;
    public override object Part2() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).max;
}