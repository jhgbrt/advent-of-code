namespace AdventOfCode.Year2017.Day19;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));
    public override object Part1() => new MazeRunner(input).Traverse().code;
    public override object Part2() => new MazeRunner(input).Traverse().steps;
}