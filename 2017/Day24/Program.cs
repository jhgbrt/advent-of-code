using System.Collections.Immutable;

using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static ImmutableList<Component> components = (
        from line in File.ReadLines("input.txt")
        select Component.Parse(line)
    ).ToImmutableList();
    internal static Result Part1() => Run(() => Bridge.Strongest(components));
    internal static Result Part2() => Run(() => Bridge.Longest(components).strength);
}

