using System.Security.Cryptography;

using static AdventOfCode.Year2015.Day04.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2015.Day04
{
    partial class AoC
    {
        static string key = "bgvyzdsv";
        internal static Result Part1() => Run(() => Solve(key, 5));
        internal static Result Part2() => Run(() => Solve(key, 6));

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

}