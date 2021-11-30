using System.Security.Cryptography;

using static AdventOfCode.Year2016.Day05.AoC201605;

namespace AdventOfCode.Year2016.Day05;

public class Tests
{
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
}