namespace AdventOfCode.Year2017.Day14;

public class AoC201714
{
    static string key = "hxtvlmkl";
    public object Part1() => Defrag.CountBitsInGrid(key);
    public object Part2() => Defrag.CountRegions(key);

}