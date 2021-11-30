namespace AdventOfCode.Year2017.Day24;

public class AoCImpl : AoCBase
{
    static ImmutableList<Component> components = (
        from line in Read.InputLines(typeof(AoCImpl))
        select Component.Parse(line)
    ).ToImmutableList();
    public override object Part1() => Bridge.Strongest(components);
    public override object Part2() => Bridge.Longest(components).strength;
}