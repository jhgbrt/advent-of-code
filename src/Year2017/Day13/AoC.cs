namespace AdventOfCode.Year2017.Day13;

public class AoCImpl : AoCBase
{
    static (int layer, int range)[] items = (
            from line in Read.InputLines(typeof(AoCImpl))
            let indexes = line.Split(": ").Select(int.Parse).ToArray()
            select (layer: indexes[0], range: indexes[1])
            ).ToArray();
    public override object Part1() => Firewall.Severity(items);

    public override object Part2() => Firewall.DelayToEscape(items);

}