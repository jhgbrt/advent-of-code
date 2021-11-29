using static AdventOfCode.Year2015.Day01.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2015.Day01
{
    partial class AoC
    {
        static string input = File.ReadAllText("input.txt");
        internal static Result Part1() => Run(() => input.Select(c => c switch { '(' => +1, ')' => -1, _ => throw new Exception() }).Sum());
        internal static Result Part2() => Run(() =>
        {
            var sum = 0;
            for (int i = 0; i < input.Length; i++)
            {
                sum += input[i] switch { '(' => +1, ')' => -1, _ => throw new Exception() };
                if (sum == -1) return i + 1;
            }
            return -1;
        });

    }
}