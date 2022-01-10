namespace AdventOfCode.Year2021.Day06;

public class AoC202106
{
    static string input = Read.InputText();
    static ImmutableArray<int> numbers = input.Split(',').Select(int.Parse).ToImmutableArray();
    public object Part1() => CountFish(80);
    public object Part2() => CountFish(256);

    private static long CountFish(int iterations)
    {
        var noffish = new long[9];
        foreach (var n in numbers)
            noffish[n]++;

        for (int i = 0; i < iterations; i++)
        {
            var age0 = noffish[0];
            for (int age = 1; age < 9; age++)
            {
                noffish[age - 1] = noffish[age];
            }
            noffish[6] += age0;
            noffish[8] = age0;
        }

        return noffish.Sum();

    }
}
