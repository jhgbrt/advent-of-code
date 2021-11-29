using System.Diagnostics;
using Xunit;
using static AdventOfCode.Year2021.Day12.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2021.Day12
{
    static class AoC
    {
        static bool test = false;
        public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        public static Result<int> Part1() => Run(1, () => -1);
        public static Result<int> Part2() => Run(2, () => -1);

        static Result<T> Run<T>(int part, Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            return new(result, sw.Elapsed);
        }
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(-1, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(-1, Part2().Value);
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);