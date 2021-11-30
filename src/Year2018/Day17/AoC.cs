namespace AdventOfCode.Year2018.Day17;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);
    public static int Part1(string[] input)
    {
        var grid = Grid.Parse(input);
        grid.Simulate();
        return grid.NofWaterReachableTiles;
    }

    public static int Part2(string[] input)
    {
        var grid = Grid.Parse(input);
        grid.Simulate();
        return grid.NofWaterTiles;
    }

}