using static AdventOfCode.Year2017.Day01.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day01
{
    partial class AoC
    {
        static bool test = false;
        public static string input = File.ReadLines(test ? "sample.txt" : "input.txt").First();

        internal static Result Part1() => Run(() => Captcha.Calculate(input, 1));
        internal static Result Part2() => Run(() => Captcha.Calculate(input, input.Length / 2));

    }
}