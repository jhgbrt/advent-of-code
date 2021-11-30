namespace AdventOfCode.Year2017.Day09;

public class AoCImpl : AoCBase
{
    public override object Part1() => new GarbageProcessor().ProcessFile("input.txt").Score;
    public override object Part2() => new GarbageProcessor().ProcessFile("input.txt").GarbageCount;
}