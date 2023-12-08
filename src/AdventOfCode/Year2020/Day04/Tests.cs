namespace AdventOfCode.Year2020.Day04;

public class Tests
{
    readonly ITestOutputHelper _output;
    public Tests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestParse()
    {
        var tr = new StringReader("byr:abc def:123\nxyz:asdf\n\nbyr:edf");
        var items = Driver.Parse(tr, s => _output.WriteLine(s)).ToList();
        Assert.Equal("asdf", items[0]["xyz"]);
        Assert.Equal("abc", items[0]["byr"]);
        Assert.Equal("123", items[0]["def"]);
        Assert.Equal("edf", items[1]["byr"]);
    }

    [Theory]
    [InlineData("", false, 0, null)]
    [InlineData("123", false, 0, null)]
    [InlineData("asb", false, 0, null)]
    [InlineData("123abc", true, 123, "abc")]
    public void AmountParse(string input, bool expected, int value, string? unit)
    {
        Assert.Equal(expected, Amount.TryParse(input, out var a));
        if (expected && a != null)
        {
            Assert.Equal(value, a.Value);
            Assert.Equal(unit, a.Unit);
        }
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("123", false)]
    [InlineData("asb", false)]
    [InlineData("123abc", false)]
    [InlineData("#", false)]
    [InlineData("#1234", false)]
    [InlineData("#123abc", true)]
    [InlineData("#789456", true)]
    [InlineData("#abcedf", true)]
    [InlineData("#abcedg", false)]
    [InlineData("#12345g", false)]
    public void ValidColor(string input, bool expected)
    {
        Assert.Equal(expected, Driver.IsValid("hcl", input));
    }

    [Theory]
    [InlineData("byr", "1919", false)]
    [InlineData("byr", "2003", false)]
    [InlineData("byr", "2002", true)]
    [InlineData("byr", "1920", true)]
    [InlineData("byr", "1970", true)]
    [InlineData("iyr", "1919", false)]
    [InlineData("iyr", "2009", false)]
    [InlineData("iyr", "2010", true)]
    [InlineData("iyr", "2020", true)]
    [InlineData("iyr", "2015", true)]
    [InlineData("eyr", "2019", false)]
    [InlineData("eyr", "2031", false)]
    [InlineData("eyr", "2020", true)]
    [InlineData("eyr", "2030", true)]
    [InlineData("eyr", "2025", true)]
    public void IsValidYear(string key, string value, bool expected)
    {
        Assert.Equal(expected, Driver.IsValid(key, value));
    }
}