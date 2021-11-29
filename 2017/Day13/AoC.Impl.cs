namespace AdventOfCode.Year2017.Day13;

partial class AoC
{
    static (int layer, int range)[] items = (
            from line in File.ReadLines("input.txt")
            let indexes = line.Split(": ").Select(int.Parse).ToArray()
            select (layer: indexes[0], range: indexes[1])
            ).ToArray();
    internal static Result Part1() => Run(() => Firewall.Severity(items));

    internal static Result Part2() => Run(() => Firewall.DelayToEscape(items));

}