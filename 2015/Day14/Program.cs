using System.Text.RegularExpressions;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static Regex regex = new Regex(@"(?<name>\w+) can fly (?<speed>\d+) km/s for (?<fly>\d+) seconds, but then must rest for (?<rest>\d+) seconds");
    static string[] input = File.ReadAllLines("input.txt"); // "sample.txt"
    static int maxtime = 2503; // 1000
    public static object Part1() => GetEntries().Select(e => e.GetDistance(maxtime)).Max();
    public static object Part2()
    {
        var entries = GetEntries();
        var points = entries.ToDictionary(e => e, e => 0);
        var tick = entries.Select(e => (entry: e, distance: 0)).ToList();
        for (int t = 1; t <= maxtime; t++)
        {
            tick = (
                from e in tick
                select (e.entry, e.entry.GetDistance(t))
                ).ToList();

            var winners = (
                from e in tick
                group e by e.distance into g
                orderby g.Key descending
                select g
                ).First();

            foreach (var winner in winners)
                points[winner.entry] += 1;

        }

        return points.Max(x => x.Value);
    }

    static IEnumerable<Entry> GetEntries() => from line in input
                                              let match = regex.Match(line)
                                              let name = match.Groups["name"].Value
                                              let speed = int.Parse(match.Groups["speed"].Value)
                                              let fly = int.Parse(match.Groups["fly"].Value)
                                              let rest = int.Parse(match.Groups["rest"].Value)
                                              select new Entry(name, speed, fly, rest);
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(2660, Part1());
    [Fact]
    public void Test2() => Assert.Equal(1256, Part2());
}

record Entry(string name, int speed, int fly, int rest)
{
    public int GetDistance(int time)
    {
        var r = time % (fly + rest);
        var n = time / (fly + rest);
        var t = n * fly + Math.Min(fly, r);
        var d = t * speed;
        return d;
    }
}