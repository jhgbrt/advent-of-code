using System.Diagnostics;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    public static Result<int> Part1() => Run(1, () => input.Count(IsValidPassword1));
    public static Result<int> Part2() => Run(2, () => input.Count(IsValidPassword2));

    static Result<T> Run<T>(int part, Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return new(result, sw.Elapsed);
    }
    private static bool IsValidPassword1(string line)
    {
        var words = line.Split(' ');
        return words.Length == words.Distinct().Count();
    }
    private static bool IsValidPassword2(string line)
    {
        var words = line.Split(' ').Select(w => new string(w.OrderBy(c => c).ToArray())).ToArray();
        return words.Length == words.Distinct().Count();
    }

}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(451, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(223, Part2().Value);
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);
