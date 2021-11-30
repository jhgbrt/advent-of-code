namespace AdventOfCode.Year2017.Day14;

public class AoC201714 : AoCBase
{
    static string key = "hxtvlmkl";
    public override object Part1() => Defrag.CountBitsInGrid(key);
    public override object Part2() => Defrag.CountRegions(key);

}