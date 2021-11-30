namespace AdventOfCode.Year2020.Day03;

public class Tests
{
    ITestOutputHelper _output;

    public Tests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(7, Driver.Part1("sample.txt"));
    }

    [Fact]
    public void GetTreesTest()
    {
        var input =
            "#.#\r\n" +
            ".#.";
        var expected = new[] { (0, 0), (2, 0), (1, 1) };
        var trees = input.Split(Environment.NewLine).GetTrees().ToList();
        Assert.Equal(expected, trees);
    }

    [Fact]
    public void TestPath()
    {
        var p = Driver.Path((3, 1)).Take(4);
        Assert.Equal(new[] { (0, 0), (3, 1), (6, 2), (9, 3) }, p);
    }
    [Fact]
    public void TestPart2()
    {
        var result = Driver.Part2("sample.txt");
        Assert.Equal(336, result);
    }
}