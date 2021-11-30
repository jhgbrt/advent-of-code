namespace AdventOfCode.Year2017.Day25;

public class AoCImpl : AoCBase
{
    static string input = Read.InputText(typeof(AoCImpl));
    public override object Part1() => Read.InputText(typeof(AoCImpl)).EncodeToSomethingSimpler().CalculateChecksum();
    public override object Part2() => -1;
}