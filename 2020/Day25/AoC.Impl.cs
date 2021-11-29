namespace AdventOfCode.Year2020.Day25;

partial class AoC
{
    public static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() =>
    {
        var (key1, key2) = (2084668L, 3704642L);
        long prime = 20201227, value = 1, result = 1;
        while (value != key2)
        {
            (value, result) = (value * 7 % prime, result * key1 % prime);
        }
        return result;
    });
    internal static Result Part2() => Run(() => -1);

}