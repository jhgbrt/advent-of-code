namespace AdventOfCode.Year2017.Day15;

public class AoC201715
{
    static int seedA = 722;
    static int seedB = 354;
    public object Part1() => Generator.GetNofMatches(seedA, seedB, 40_000_000);
    public object Part2() => Generator.GetNofMatches(seedA, seedB, 5_000_000, 4, 8);

}