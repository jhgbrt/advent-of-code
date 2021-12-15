namespace AdventOfCode.Year2017.Day25;

public class AoC201725
{
    static string input = Read.InputText();
    public object Part1() => Read.InputText().EncodeToSomethingSimpler().CalculateChecksum();
    public object Part2() => -1;
}