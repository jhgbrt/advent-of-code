namespace AdventOfCode.Year2017.Day11;

public class AoC201711
{
    static string[] input = Read.InputLines(typeof(AoC201711));
    public object Part1() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).distance;
    public object Part2() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).max;
}