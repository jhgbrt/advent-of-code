using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2018.Day23;

public class AoC201823
{
    static string[] input = Read.SampleLines();
    public object Part1()
    {
        return -1;
    }
    public object Part2() => -1;
}

readonly record struct Bot(int X, int Y, int Z, int r)
{
    static Regex regex = new Regex(@"pos=<(?<X>\d+),(?<Y>\d+),(<?Z>\d+)>, r=(<?r>\d+)", RegexOptions.Compiled);
    public static Bot? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Bot(
            int.Parse(match.Groups["X"].Value),
            int.Parse(match.Groups["Y"].Value),
            int.Parse(match.Groups["Z"].Value),
            int.Parse(match.Groups["r"].Value)) : null;
    }
}
