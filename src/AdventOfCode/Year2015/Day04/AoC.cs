using AdventOfCode.Common;

namespace AdventOfCode.Year2015.Day04;

public class AoC201504
{
    static string key = "bgvyzdsv";
    public object Part1() => Solve(key, 5);
    public object Part2() => Solve(key, 6);

    internal static int Solve(string key, int n)
    {
        var i = 0;
        while (true)
        {
            var hash = MD5Hash.Compute(key + i);
            if (hash.Take(n).All(x => x == '0'))
            {
                return i;
            }
            i++;
        }
    }
}