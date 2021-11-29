namespace AdventOfCode.Year2017.Day09;

public class Tests
{

    [Theory]
    [InlineData("<>", 0)]
    [InlineData("<random characters>", 0)]
    [InlineData("<<<<>", 0)]
    [InlineData("<{!>}>", 0)]
    [InlineData("<!!>", 0)]
    [InlineData("<!!!>>", 0)]
    [InlineData("<{o\"i!a,<{i<a>", 0)]
    public void MostlyGarbage(string input, int expected)
    {
        var processor = new GarbageProcessor();
        var result = processor.Process(input);
        Assert.Equal(expected, result.Score);
    }

    [Theory]
    [InlineData("<>", 0)]
    [InlineData("<random characters>", 17)]
    [InlineData("<<<<>", 3)]
    [InlineData("<{!>}>", 2)]
    [InlineData("<!!>", 0)]
    [InlineData("<!!!>>", 0)]
    [InlineData("<{o\"i!a,<{i<a>", 10)]
    public void GarbageCount(string input, int expected)
    {
        var processor = new GarbageProcessor();
        var result = processor.Process(input);
        Assert.Equal(expected, result.GarbageCount);
    }

    [Theory]
    [InlineData("{}", 1)]
    [InlineData("{{{}}}", 3)]
    [InlineData("{{},{}}", 3)]
    [InlineData("{{{},{},{{}}}}", 6)]
    [InlineData("{<{},{},{{}}>}", 1)]
    [InlineData("{<a>,<a>,<a>,<a>}", 1)]
    [InlineData("{{<a>},{<a>},{<a>},{<a>}}", 5)]
    [InlineData("{{<!>},{<!>},{<!>},{<a>}}", 2)]
    public void Groups(string input, int expected)
    {
        var processor = new GarbageProcessor();
        var result = processor.Process(input);
        Assert.Equal(expected, result.Groups);
    }

    [Theory]
    [InlineData("{}", 1)]
    [InlineData("{{{}}}", 6)]
    [InlineData("{{},{}}", 5)]
    [InlineData("{{{},{},{{}}}}", 16)]
    [InlineData("{{<ab>},{<ab>},{<ab>},{<ab>}}", 9)]
    [InlineData("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9)]
    [InlineData("{{<a!>},{<a!>},{<a!>},{<ab>}}", 3)]
    public void Score(string input, int expected)
    {
        var processor = new GarbageProcessor();
        var result = processor.Process(input);
        Assert.Equal(expected, result.Score);
    }
}
