namespace AdventOfCode.Year2017.Day19;

public class AoC201719
{
    static string[] input = Read.InputLines();
    public object Part1() => new MazeRunner(input).Traverse().code;
    public object Part2() => new MazeRunner(input).Traverse().steps;
}