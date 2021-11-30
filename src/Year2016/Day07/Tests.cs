using static AdventOfCode.Year2016.Day07.AoC201607;

namespace AdventOfCode.Year2016.Day07;

public class Tests
{
    [Theory]
    [InlineData("abba[mnop]qrst", true)]
    [InlineData("abcd[bddb]xyyx", false)]
    [InlineData("aaaa[qwer]tyui", false)]
    [InlineData("ioxxoj[asdfgh]zxcvbn", true)]
    [InlineData("ioxxoj[asdfgh]zxcvbnioxxoj[asdfgh]zxcvbn", true)]
    [InlineData("ioxxoj[asdfgh]zxcvbnioxxoj[asabba]zxcvbn", false)]
    public void SpecsTLS(string input, bool expected)
    {
        Assert.Equal(expected, new IPAddress(input).SupportsTLS());
    }



    [Theory]
    [InlineData("aba[bab]xyz", true)]
    [InlineData("xyx[xyx]xyx", false)]
    [InlineData("aaa[kek]eke", true)]
    [InlineData("aaa[aaa]eke", false)]
    [InlineData("zazbz[bzb]cdb", true)]
    public void SpecsSSL(string input, bool expected)
    {
        Assert.Equal(expected, new IPAddress(input).SupportsSSL());
    }
}