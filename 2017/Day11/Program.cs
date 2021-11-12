using hexgrid;
using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");
    internal static Result Part1() => Run(() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).distance);
    internal static Result Part2() => Run(() => HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).max);
}
