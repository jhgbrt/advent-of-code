namespace AdventOfCode.Year2018.Day07;

public class Specs
{
    string[] input = new[]
    {
            "Step C must be finished before step A can begin.",
            "Step C must be finished before step F can begin.",
            "Step A must be finished before step B can begin.",
            "Step A must be finished before step D can begin.",
            "Step B must be finished before step E can begin.",
            "Step D must be finished before step E can begin.",
            "Step F must be finished before step E can begin."
        };

    [Fact]
    public void ToEdge()
    {
        var edge = "Step C must be finished before step A can begin.".ToEdge();
        Assert.Equal('C', edge.from);
        Assert.Equal('A', edge.to);
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoC201807.Part1(input);

        Assert.Equal("CABDFE", result);
    }

    [Fact]
    public void GetTime()
    {
        Assert.Equal(61, 'A'.GetTime(60));
        Assert.Equal(27, 'Z'.GetTime(1));
    }

    [Fact]
    public void TestPart2()
    {
        var result = input.ToGraph().FindTotalDuration(2, 0);
        Assert.Equal(15, result);
    }
}
