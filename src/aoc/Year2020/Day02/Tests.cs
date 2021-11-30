namespace AdventOfCode.Year2020.Day02;

public class Tests
{
    [Fact]
    public void TestRegex()
    {
        var matches = Driver.LineRegex.Match("123-456 x: asdfasdf");
        Assert.True(matches.Success);
        Assert.Equal("123", matches.Groups["Min"].Value);
        Assert.Equal("456", matches.Groups["Max"].Value);
        Assert.Equal("x", matches.Groups["Letter"].Value);
        Assert.Equal("asdfasdf", matches.Groups["Password"].Value);
    }

    [Theory]
    [InlineData("1-3 a: abcde", true)]
    [InlineData("1-3 a: aaaaa", false)]
    [InlineData("1-3 a: bcde", false)]
    public void Rule1Test(string input, bool expected)
    {
        Assert.Equal(expected, Driver.IsValid1(Driver.ToEntry(input)));
    }
    [Theory]
    [InlineData("1-3 a: abcde", true)]
    [InlineData("1-3 a: cbade", true)]
    [InlineData("1-3 a: ddddd", false)]
    [InlineData("1-3 c: ccccc", false)]
    public void Rule2Test(string input, bool expected)
    {
        Assert.Equal(expected, Driver.IsValid1(Driver.ToEntry(input)));
    }

    [Fact]
    public void TestPart1()
    {
        var result = Driver.Part1("sample.txt");
        Assert.Equal(2, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = Driver.Part2("sample.txt");
        Assert.Equal(1, result);
    }
}