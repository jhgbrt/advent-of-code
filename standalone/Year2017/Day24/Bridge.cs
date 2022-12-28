namespace AdventOfCode.Year2017.Day24;

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
