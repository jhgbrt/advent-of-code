namespace AdventOfCode.Year2020.Day11;

public class Tests
{
    Grid example = Grid.Parse(Read.SampleLines(typeof(AoCImpl)));
    [Fact]
    public void Test1()
    {
        Assert.Equal(37, example.Handle(Grid.Rule1));
    }
    [Fact]
    public void Test2()
    {
        Assert.Equal(26, example.Handle(Grid.Rule2));
    }

}