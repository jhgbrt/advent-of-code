namespace AdventOfCode.Year2017.Day02;

public class AoC201702 : AoCBase
{
    public static string input = Read.InputText(typeof(AoC201702));

    public override object Part1() => CheckSum.CheckSum1(new StringReader(input));
    public override object Part2() => CheckSum.CheckSum2(new StringReader(input));

}