using static AdventOfCode.Year2017.Day03.Spiral;

namespace AdventOfCode.Year2017.Day03;

partial class AoC
{
    public const int input = 265149;

    internal static Result Part1() => Run(() => DistanceToOrigin(input));
    internal static Result Part2() => Run(() => SpiralValues().SkipWhile(i => i.value <= input).First().value);

}