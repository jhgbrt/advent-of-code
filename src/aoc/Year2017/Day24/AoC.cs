namespace AdventOfCode.Year2017.Day24;

public class AoC201724
{
    static ImmutableList<Component> components = (
        from line in Read.InputLines(typeof(AoC201724))
        select Component.Parse(line)
    ).ToImmutableList();
    public object Part1() => Bridge.Strongest(components);
    public object Part2() => Bridge.Longest(components).strength;
}