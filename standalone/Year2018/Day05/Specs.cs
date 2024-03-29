namespace AdventOfCode.Year2018.Day05;

public class Specs
{
    private readonly string input = "dabAcCaCBAcCcaDA";

    [Fact]
    public void TestPart1()
    {
        var result = AoC201805.Part1(input);
        Assert.Equal(10, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoC201805.Part2(input);
        Assert.Equal(4, result);
    }
}
