using static System.Math;

namespace AdventOfCode.Year2019.Day03;

public class AoC201903 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201903));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);
    public static int Part1(string[] input)
    {
        var points = input[0].Points().ToHashSet();
        var crossings = input[1].Points().Where(p => points.Contains(p));
        return crossings.Min(x => Abs(x.x) + Abs(x.y));
    }

    public static int Part2(string[] input)
    {
        var points1 = input[0].Points().Select((p, i) => (p, steps: i + 1)).ToLookup(x => x.p, x => x.steps);
        var points2 = input[1].Points().Select((p, i) => (p, steps: i + 1)).ToLookup(x => x.p, x => x.steps);
        var crossings = points2.Select(p => p.Key).Where(p => points1.Contains(p));

        var steps = crossings
            .Select(p => points1[p].Min() + points2[p].Min())
            .Min();

        return steps;
    }
}