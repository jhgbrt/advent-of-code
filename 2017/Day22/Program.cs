using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadLines("input.txt").ToArray();
    internal static Result Part1() => Run(() =>
        {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid(10000);
    });

    internal static Result Part2() => Run(() =>
        {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid2(10000000);
    });

}

