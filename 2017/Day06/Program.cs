using System.Collections.Immutable;
using System.Diagnostics;

using Xunit;

using static AoC;

Console.WriteLine(Day06());

static class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    public static Result<(int part1,int part2)> Day06() => Run(1, () => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()));

    static Result<T> Run<T>(int part, Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return new(result, sw.Elapsed);
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal((14029, 2765), Day06().Value);
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);

