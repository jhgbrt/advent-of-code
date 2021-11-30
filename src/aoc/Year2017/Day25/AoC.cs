namespace AdventOfCode.Year2017.Day25;

public class AoC201725 : AoCBase
{
    static string input = Read.InputText(typeof(AoC201725));
    public override object Part1() => Read.InputText(typeof(AoC201725)).EncodeToSomethingSimpler().CalculateChecksum();
    public override object Part2() => -1;
}