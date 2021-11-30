namespace AdventOfCode.Year2018.Day20;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);
    public static int Part1(string[] input) => input.Single().Distances().Max();

    public static int Part2(string[] input) => input.Single().Distances().Where(i => i >= 1000).Count();
}