using Microsoft.CodeAnalysis;

namespace AdventOfCode.Year2018.Day23;

public class AoC201823
{
    static string[] input = Read.SampleLines();

    static Bot[] bots = (from line in input
                         let bot = Bot.TryParse(line)
                         where bot.HasValue select bot.Value).ToArray();
    static Bot strongest = bots.MaxBy(b => b.range);
    public int Part1() => bots.Count(strongest.InRange);

    public object Part2()
    {
        var q = from bot1 in bots
                let intersections = (
                    from i in bots
                    where i.IntersectsWith(bot1)
                    select i
                    ).ToList()
                orderby intersections.Count descending
                select (bot1, intersections);
 
        foreach (var (b,i) in q)
        {
            Console.WriteLine($"{b} intersects with {i.Count} bots:");
            foreach (var o in i) Console.WriteLine($"   {o} - {o.IntersectsWith(b)}");
        }



        return 0;
    }
}

readonly record struct Bot(Pos position, long range)
{
    static Regex regex = new (@"pos=<(?<X>[-\d]+),(?<Y>[-\d]+),(?<Z>[-\d]+)>, r=(?<r>[-\d]+)", RegexOptions.Compiled);
    public static Bot? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Bot(new(
            long.Parse(match.Groups["X"].Value),
            long.Parse(match.Groups["Y"].Value),
            long.Parse(match.Groups["Z"].Value)),
            long.Parse(match.Groups["r"].Value)
            ) : null;
    }
    public bool InRange(Bot other) => other.position.Distance(position) <= range;
    public bool IntersectsWith(Bot other) => position.Distance(other.position) <= range + other.range;
    public IEnumerable<Pos> Outer()
    {
        yield break;
    }
}

readonly record struct Pos(long X, long Y, long Z)
{
    public long Distance(Pos other) => Abs(X - other.X) + Abs(Y - other.Y) + Abs(Z - other.Z);
}

readonly record struct Search(Pos origin, long range);
