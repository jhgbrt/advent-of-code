using Xunit;

public class Tests
{
    [Theory]
    [InlineData("", 0)]
    [InlineData("1122", 3)]
    [InlineData("1111", 4)]
    [InlineData("1234", 0)]
    [InlineData("91212129", 9)]
    public void Test1(string input, int expected)
    {
        Assert.Equal(expected, Captcha.Calculate(input, 1));
    }
    [Theory]
    [InlineData("1212", 6)]
    [InlineData("1221", 0)]
    [InlineData("123425", 4)]
    [InlineData("123123", 12)]
    [InlineData("12131415", 4)]
    public void Test2(string input, int expected)
    {
        Assert.Equal(expected, Captcha.Calculate(input, input.Length / 2));
    }
}
