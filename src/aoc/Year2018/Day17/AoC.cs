namespace AdventOfCode.Year2018.Day17;

public class AoC201817
{
    static string[] input = Read.InputLines(typeof(AoC201817));

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
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