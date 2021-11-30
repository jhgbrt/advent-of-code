namespace AdventOfCode.Year2020.Day01;

public class Tests
{
    [Fact]
    public void TestPart1()
    {
        var numbers = "sample.txt".LinesToNumbers().ToList();
        var result = numbers.Part1();
        Assert.Equal(514579, result);
    }

    [Fact]
    public void AllNumbersAreUnique()
    {
        var numbers = "input.txt".LinesToNumbers().ToList();
        Assert.Equal(numbers.Count, numbers.Distinct().Count());
    }

    [Fact]
    public void TestPart2()
    {
        var numbers = "sample.txt".LinesToNumbers().ToList();
        var result = numbers.Part2();
        Assert.Equal(241861950, result);
    }
}