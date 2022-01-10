namespace AdventOfCode.Year2017.Day01;

public class AoC201701
{
    public static string input = Read.InputLines().First();

    public object Part1() => Captcha.Calculate(input, 1);
    public object Part2() => Captcha.Calculate(input, input.Length / 2);

}