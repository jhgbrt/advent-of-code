namespace AdventOfCode.Year2017.Day10;

partial class AoC
{
    static string input = "206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3";
    internal static Result Part1() => Run(() =>
    {
        var result = KnotsHash.Hash(input.Split(',').Select(byte.Parse).ToArray());
        var value = result[0] * result[1];
        return value;
    });
    internal static Result Part2() => Run(() => KnotsHash.Hash(input));
}