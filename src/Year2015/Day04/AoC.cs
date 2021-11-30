using System.Security.Cryptography;

namespace AdventOfCode.Year2015.Day04;

public class AoCImpl : AoCBase
{
    static string key = "bgvyzdsv";
    public override object Part1() => Solve(key, 5);
    public override object Part2() => Solve(key, 6);

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