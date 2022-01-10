namespace AdventOfCode.Year2020.Day02;

public class AoC202002
{
    public object Part1() => Driver.Part1(Read.InputLines());
    public object Part2() => Driver.Part2(Read.InputLines());
}
record Entry(int Min, int Max, char Letter, string Password);
