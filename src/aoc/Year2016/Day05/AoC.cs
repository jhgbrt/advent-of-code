namespace AdventOfCode.Year2016.Day05;

public class AoC201605 : AoCBase
{
    public static string input = "ugkcyxxp";

    public override object Part1() => new Cracker().GeneratePassword1(input, 8);
    public override object Part2() => new Cracker().GeneratePassword2(input, 8);
}