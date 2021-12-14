namespace AdventOfCode.Year2016.Day05;

public class AoC201605
{
    public static string input = "ugkcyxxp";

    public object Part1() => new Cracker().GeneratePassword1(input, 8);
    public object Part2() => new Cracker().GeneratePassword2(input, 8);
}