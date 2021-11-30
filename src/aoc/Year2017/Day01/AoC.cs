namespace AdventOfCode.Year2017.Day01;

public class AoC201701 : AoCBase
{
    public static string input = Read.InputLines(typeof(AoC201701)).First();

    public override object Part1() => Captcha.Calculate(input, 1);
    public override object Part2() => Captcha.Calculate(input, input.Length / 2);

}