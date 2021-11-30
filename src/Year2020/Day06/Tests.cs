using static AdventOfCode.Year2020.Day06.AoCImpl;

namespace AdventOfCode.Year2020.Day06;

public class Tests
{
    [Fact]
    public void TestExample()
    {
        var example = Read.Lines(typeof(AoCImpl), "sample.txt").AsBlocks();
        var (e1, e2) = (Part1(example), Part2(example));
        Debug.Assert((e1, e2) == (11, 6), $"{(e1, e2)}");

    }
}