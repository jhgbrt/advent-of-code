using static AdventOfCode.Year2020.Day01.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day01
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");
        static int[] numbers = input.Select(int.Parse).ToArray();

        internal static Result Part1() => Run(() => numbers.Part1());
        internal static Result Part2() => Run(() => numbers.Part2());

    }
}

record Pair(int i, int j)
{
    public int Sum => i + j;
}
record Triplet(int i, int j, int k)
{
    public int Sum => i + j + k;
}

namespace AdventOfCode
{
}
