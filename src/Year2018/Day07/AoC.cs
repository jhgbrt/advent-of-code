namespace AdventOfCode.Year2018.Day07;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);

    public static string Part1(string[] input) => new string(input.ToGraph().FindStepOrder().ToArray());
    public static int Part2(string[] input) => input.ToGraph().FindTotalDuration(5, 60);
}