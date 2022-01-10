using static AdventOfCode.Year2017.Day03.Spiral;

namespace AdventOfCode.Year2017.Day03;

public class AoC201703
{
    public const int input = 265149;

    public object Part1() => DistanceToOrigin(input);
    public object Part2() => SpiralValues().SkipWhile(i => i.value <= input).First().value;

}