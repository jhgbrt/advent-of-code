namespace AdventOfCode.Year2017.Day13;

public class AoC201713
{
    static (int layer, int range)[] items = (
            from line in Read.InputLines(typeof(AoC201713))
            let indexes = line.Split(": ").Select(int.Parse).ToArray()
            select (layer: indexes[0], range: indexes[1])
            ).ToArray();
    public object Part1() => Firewall.Severity(items);

    public object Part2() => Firewall.DelayToEscape(items);

}