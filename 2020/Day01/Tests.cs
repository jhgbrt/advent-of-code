namespace AdventOfCode.Year2020.Day01;

public class Tests
{
    [Fact]
    async Task TestPart1()
    {
        var numbers = (await "sample.txt".LinesToNumbers()).ToList();
        var result = numbers.Part1();
        Assert.Equal(514579, result);
    }

    [Fact]
    public async Task AllNumbersAreUnique()
    {
        var numbers = (await "input.txt".LinesToNumbers()).ToList();
        Assert.Equal(numbers.Count, numbers.Distinct().Count());
    }

    [Fact]
    async Task TestPart2()
    {
        var numbers = (await "sample.txt".LinesToNumbers()).ToList();
        var result = numbers.Part2();
        Assert.Equal(241861950, result);
    }
}