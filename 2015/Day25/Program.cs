using System.Diagnostics;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");
    const int row = 3010;
    const int column = 3019;
    const long code = 20151125;
    const long m = 252533;
    const long d = 33554393;

    public static Result<long> Part1() => Run(1, () =>
    {
        var value = code;
        (var r, var c) = (1, 1);
        while (true)
        {
            (r, c) = (r - 1, c + 1);
            if (r == 0) (r, c) = (c, 1);
            value = (m * value) % d;
            if ((r, c) == (row, column)) return value;
        }
    });
    public static Result<int> Part2() => Run(2, () => -1);

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
    public void Test1() => Assert.Equal(8997277, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(-1, Part2().Value);
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);
