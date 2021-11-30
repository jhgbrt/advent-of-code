using static AdventOfCode.Year2020.Day08.AoCImpl;

namespace AdventOfCode.Year2020.Day08;

public class Tests
{
    public void Test1() => Assert.Equal(5, ReadProgram("example.txt").Part1());
    public void Test2() => Assert.Equal(8, ReadProgram("example.txt").Part2());
}