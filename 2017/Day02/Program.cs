using static AdventOfCode.Year2017.Day02.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day02
{
    partial class AoC
    {
        static bool test = false;
        public static string input = File.ReadAllText(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => CheckSum.CheckSum1(new StringReader(input)));
        internal static Result Part2() => Run(() => CheckSum.CheckSum2(new StringReader(input)));

    }


}