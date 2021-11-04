using System.Diagnostics;
using System.Text;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static bool test = false;
    static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    public static Result<string> Part1() => Run(1, () => new Accumulator().Decode(input, 8, false));
    public static Result<string> Part2() => Run(2, () => new Accumulator().Decode(input, 8, true));

    static Result<T> Run<T>(int part, Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return new(result, sw.Elapsed);
    }
}

public class Tests
{
    static string[] input = File.ReadAllLines("sample.txt");
    [Fact]
    public void Test1() => Assert.Equal("kjxfwkdh", Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal("xrwcsnps", Part2().Value);
    [Fact]
    public void Part1Test()
    {
        var result = new Accumulator().Decode(input, 6);
        Assert.Equal("easter", result);
    }

    [Fact]
    public void Part2Test()
    {
        var result = new Accumulator().Decode(input, 6, true);
        Assert.Equal("advent", result);
    }
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);


public class Accumulator
{
    public string Decode(IEnumerable<string> data, int lineLength, bool ascending = false)
    {
        var query = from line in data
                    from item in line.Select((c, i) => new { c, pos = i })
                    select item;

        var lookup = query.ToLookup(item => item.pos);

        var sb = new StringBuilder();
        for (int i = 0; i < lineLength; i++)
        {
            var g = lookup[i];
            var grpByChar = g.GroupBy(item => item.c);
            var ordered = ascending
                ? grpByChar.OrderBy(x => x.Count())
                : grpByChar.OrderByDescending(x => x.Count());
            var c = ordered.First().First().c;
            sb.Append(c);
        }
        return sb.ToString();
    }
}
