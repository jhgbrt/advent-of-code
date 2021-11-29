using System.Numerics;

using static AdventOfCode.Year2015.Day24.AoC;

namespace AdventOfCode.Year2015.Day24;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(new BigInteger(11846773891), Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(new BigInteger(80393059), Part2().Value);

}