namespace AdventOfCode.Year2018.Day20;

public class AoC201820
{
    static string[] input = Read.InputLines(typeof(AoC201820));

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
    public static int Part1(string[] input) => input.Single().Distances().Max();

    public static int Part2(string[] input) => input.Single().Distances().Where(i => i >= 1000).Count();
}