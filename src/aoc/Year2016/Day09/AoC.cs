namespace AdventOfCode.Year2016.Day09;

public class AoC201609 : AoCBase
{
    public static string input = Read.InputLines(typeof(AoC201609)).First();

    public override object Part1() => input.GetDecompressedSize(0);
    public override object Part2() => input.GetDecompressedSize2(0, input.Length);

}