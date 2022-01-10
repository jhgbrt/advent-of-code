namespace AdventOfCode.Year2016.Day09;

public class AoC201609
{
    public static string input = Read.InputLines().First();

    public object Part1() => input.GetDecompressedSize(0);
    public object Part2() => input.GetDecompressedSize2(0, input.Length);

}