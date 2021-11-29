using static AdventOfCode.Year2017.Day16.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day16
{
    partial class AoC
    {
        static Dancer dancer = new Dancer(new StreamReader("input.txt"));
        static string initial = "abcdefghijklmnop";
        internal static Result Part1() => Run(() => dancer.Run(initial));
        internal static Result Part2() => Run(() => dancer.Run(initial, 1000000000));

    }
}