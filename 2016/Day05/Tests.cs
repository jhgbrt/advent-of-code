using System.Security.Cryptography;

using static AdventOfCode.Year2016.Day05.AoC;

namespace AdventOfCode.Year2016.Day05;

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal("d4cd2ee1", Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal("f2c730e5", Part2().Value);

    [Theory]
    [InlineData(0, 0, 0, 0, true)]
    [InlineData(0, 0, 0, 0xFF, true)]
    [InlineData(0, 0, 0x0F, 0xFF, true)]
    [InlineData(0, 0, 0x1F, 0xFF, false)]
    public void ByteArrayStartsWith5ZeroesTest(byte a, byte b, byte c, byte d, bool expected)
    {
        var cracker = new Cracker();
        var result = cracker.StartsWith5Zeroes(new[] { a, b, c, d });
        Assert.Equal(expected, result);
    }

    MD5 _md5 = MD5.Create();

    [Theory]
    [InlineData("abc0", false)]
    [InlineData("abc1", false)]
    [InlineData("abc3231929", true)]
    [InlineData("abc5017308", true)]
    [InlineData("abc5278568", true)]
    public void TestHashValidation(string input, bool expected)
    {
        var cracker = new Cracker();
        var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(input));
        var isValid = cracker.StartsWith5Zeroes(hash);
        Assert.Equal(expected, isValid);
    }

    [Fact]
    public void TestPart1()
    {
        var cracker = new Cracker();
        var password = cracker.GeneratePassword1("abc", 8);
        Assert.Equal("18f47a30", password);
    }
}