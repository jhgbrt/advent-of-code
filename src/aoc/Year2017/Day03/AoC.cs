using static AdventOfCode.Year2017.Day03.Spiral;

namespace AdventOfCode.Year2017.Day03;

public class AoC201703 : AoCBase
{
    public const int input = 265149;

    public override object Part1() => DistanceToOrigin(input);
    public override object Part2() => SpiralValues().SkipWhile(i => i.value <= input).First().value;

}