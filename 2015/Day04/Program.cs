using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static string key = "bgvyzdsv";
    public static object Part1() => Solve(key, 5);
    public static object Part2() => Solve(key, 6);
    internal static int Solve(string key, int n)
    {
        var md5 = MD5.Create();
        var i = 0;
        while (true)
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key + i));
            var s = Convert.ToHexString(hash);
            if (s.Take(n).All(x => x == '0'))
            {
                return i;
            }
            i++;
        }
    }
}


public class Tests
{
    [Theory]
    [InlineData("abcdef", 609043)]
    [InlineData("pqrstuv", 1048970)]
    public void Test(string input, int expected)
    {
        var result = Solve(input, 5);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Test1() => Assert.Equal(254575, Part1());
    [Fact]
    public void Test2() => Assert.Equal(1038736, Part2());

}