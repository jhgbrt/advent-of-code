namespace AdventOfCode.Year2021.Day06;

public class AoC202106 : AoCBase
{
    static string input = Read.InputText (typeof(AoC202106));
    static ImmutableArray<int> numbers = input.Split(',').Select(int.Parse).ToImmutableArray();
    public override object Part1() => CountFish(80);
    public override object Part2() => CountFish(256);

    private static long CountFish(int iterations)
    {
        var noffish = new long[9];
        foreach (var n in numbers)
            noffish[n]++;

        for (int i = 0; i < iterations; i++)
        {
            var age6 = noffish[0];
            for (int age = 1; age < 9; age++)
            {
                noffish[age - 1] = noffish[age];
            }
            noffish[6] += age6;
            noffish[8] = age6;
        }

        return noffish.Sum();

    }
}
