namespace AdventOfCode.Year2019.Day05;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => Part1(input).Last();
    public override object Part2() => string.Join(",", Part2(input));
    public static IEnumerable<int> Part1(string[] program) => IntCode.Run(Parse(program), 1);
    public static IEnumerable<int> Part1(string[] program, int input) => IntCode.Run(Parse(program), input);
    public static IEnumerable<int> Part2(string[] program) => IntCode.Run(Parse(program), 5);

    static ImmutableArray<int> Parse(string[] input) => input[0].Split(',').Select(int.Parse).ToImmutableArray();


}