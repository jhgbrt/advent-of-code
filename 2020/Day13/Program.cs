using static AoC;

using P1 = Part1;
using P2 = Part2;

Console.WriteLine(AoC.Part1());
Console.WriteLine(AoC.Part2());

partial class AoC
{
    internal static Result Part1() => Run(() => P1.Run());
    internal static Result Part2() => Run(() => P2.Run());
}