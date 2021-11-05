using System.Diagnostics;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static bool test = false;
    public static string input = File.ReadAllText(test ? "sample.txt" : "input.txt");

    public static Result<int> Part1() => Run(1, () => CheckSum.CheckSum1(new StringReader(input)));
    public static Result<int> Part2() => Run(2, () => CheckSum.CheckSum2(new StringReader(input)));

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
    public void Test1() => Assert.Equal(45972, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(326, Part2().Value);

    [Theory]
    [InlineData("5\t1\t9\t5\r\n" +
                "7\t5\t3\r\n" +
                "2\t4\t6\t8\r\n", 18)]
    public void TestPart1(string input, int checksum)
    {
        Assert.Equal(checksum, CheckSum.CheckSum1(new StringReader(input)));
    }
    [Theory]
    [InlineData("5\t9\t2\t8\r\n" +
                "9\t4\t7\t3\r\n" +
                "3\t8\t6\t5\r\n", 9)]
    public void TestPart2(string input, int checksum)
    {
        Assert.Equal(checksum, CheckSum.CheckSum2(new StringReader(input)));
    }
}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);
