namespace AdventOfCode.Year2017.Day06;

public class AoC201706
{
    
    public static string[] input = Read.InputLines(typeof(AoC201706));

    public object Part1() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).steps;
    public object Part2() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).loopSize;

}