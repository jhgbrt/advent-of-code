namespace AdventOfCode.Year2018.Day01;

public class Specs
{
    string[] input = new[]
    {
            "+1",
            "-2",
            "+3",
            "+1"
        };

    [Fact]
    public void TestPart1()
    {
        var result = AoCImpl.Part1(input);
        Assert.Equal(3, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoCImpl.Part2(input);
        Assert.Equal(2, result);
    }
}
