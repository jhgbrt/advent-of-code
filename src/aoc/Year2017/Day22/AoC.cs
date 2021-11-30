namespace AdventOfCode.Year2017.Day22;

public class AoC201722 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201722)).ToArray();
    public override object Part1()
    {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid(10000);
    }

    public override object Part2()
    {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid2(10000000);
    }

}