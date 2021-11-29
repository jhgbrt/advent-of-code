using static AdventOfCode.Year2017.Day25.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day25
{
    partial class AoC
    {
        static string input = File.ReadAllText("input.txt");
        internal static Result Part1() => Run(() => File.ReadAllText("input.txt").EncodeToSomethingSimpler().CalculateChecksum());
        internal static Result Part2() => Run(() => -1);
    }
}