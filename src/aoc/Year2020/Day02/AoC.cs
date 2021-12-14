namespace AdventOfCode.Year2020.Day02;

public class AoC202002
{
    public object Part1() => Driver.Part1("input.txt");
    public object Part2() => Driver.Part2("input.txt");
}
record Entry(int Min, int Max, char Letter, string Password);
