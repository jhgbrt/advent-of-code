namespace AdventOfCode.Year2017.Day09;

public class AoC201709
{
    public object Part1() => new GarbageProcessor().ProcessFile("input.txt").Score;
    public object Part2() => new GarbageProcessor().ProcessFile("input.txt").GarbageCount;
}