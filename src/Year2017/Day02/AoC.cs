namespace AdventOfCode.Year2017.Day02;

public class AoCImpl : AoCBase
{
    public static string input = Read.InputText(typeof(AoCImpl));

    public override object Part1() => CheckSum.CheckSum1(new StringReader(input));
    public override object Part2() => CheckSum.CheckSum2(new StringReader(input));

}