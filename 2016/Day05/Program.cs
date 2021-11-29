
using static AdventOfCode.Year2016.Day05.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day05
{
    partial class AoC
    {
        public static string input = "ugkcyxxp";

        internal static Result Part1() => Run(() => new Cracker().GeneratePassword1(input, 8));
        internal static Result Part2() => Run(() => new Cracker().GeneratePassword2(input, 8));
    }
}