namespace AdventOfCode.Year2020.Day03;

public class Tests
{
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
}