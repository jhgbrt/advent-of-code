namespace AdventOfCode.Year2017.Day24;

public class AoC201724 : AoCBase
{
    static ImmutableList<Component> components = (
        from line in Read.InputLines(typeof(AoC201724))
        select Component.Parse(line)
    ).ToImmutableList();
    public override object Part1() => Bridge.Strongest(components);
    public override object Part2() => Bridge.Longest(components).strength;
}