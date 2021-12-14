namespace AdventOfCode.Year2017.Day25;

public class AoC201725
{
    static string input = Read.InputText(typeof(AoC201725));
    public object Part1() => Read.InputText(typeof(AoC201725)).EncodeToSomethingSimpler().CalculateChecksum();
    public object Part2() => -1;
}