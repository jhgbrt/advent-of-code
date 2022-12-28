namespace AdventOfCode.Year2017.Day24;

public class AoC201724
{
    static ImmutableList<Component> components = (
        from line in Read.InputLines()
        select Component.Parse(line)
    ).ToImmutableList();
    public object Part1() => Bridge.Strongest(components);
    public object Part2() => Bridge.Longest(components).strength;
}

static class Bridge
{
    public static int Strongest(ImmutableList<Component> components, int pins = 0, int strength = 0)
        => (
            from component in components
            where component.Matches(pins)
            select Strongest(components.Remove(component), component.Other(pins), strength + component.Strength)
        ).Concat(new[] { strength })
        .Max();

    public static (int strength, int length) Longest(IImmutableList<Component> components, int pins = 0, int strength = 0, int length = 0)
        => (
                from component in components
                where component.Matches(pins)
                select Longest(components.Remove(component), component.Other(pins), strength + component.Strength, length + 1)
            ).Concat(new[] { (strength: strength, length: length) })
            .OrderByDescending(x => x.length)
            .ThenByDescending(x => x.strength)
            .First();
}

readonly record struct Component(int Port1, int Port2)
{
    public int Smallest => Min(Port1, Port2);
    public bool Matches(int pins) => pins == Port1 || pins == Port2;
    public int Other(int pins) => pins == Port1 ? Port2 : Port1;
    public int Strength => Port1 + Port2;
    public override string ToString() => $"{Port1}/{Port2}";

    public static Component Parse(string s)
    {
        var split = s.Split('/');
        return new Component(int.Parse(split[0]), int.Parse(split[1]));
    }
}