namespace AdventOfCode.Year2017.Day01;

public class AoCImpl : AoCBase
{
    public static string input = Read.InputLines(typeof(AoCImpl)).First();

    public override object Part1() => Captcha.Calculate(input, 1);
    public override object Part2() => Captcha.Calculate(input, input.Length / 2);

}