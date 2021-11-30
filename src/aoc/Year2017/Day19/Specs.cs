namespace AdventOfCode.Year2017.Day19;

public class Specs
{
    private static string SampleMaze = "     |          \r\n" +
                                       "     |  +--+    \r\n" +
                                       "     A  |  C    \r\n" +
                                       " F---|----E|--+ \r\n" +
                                       "     |  |  |  D \r\n" +
                                       "     +B-+  +--+ ";

    [Fact]
    public void Part1()
    {
        var result = new MazeRunner(SampleMaze.ReadLines().ToArray()).Traverse().code;
        Assert.Equal("ABCDEF", result);
    }
    [Fact]
    public void Part2()
    {
        var result = new MazeRunner(SampleMaze.ReadLines().ToArray()).Traverse().steps;
        Assert.Equal(38, result);
    }
}