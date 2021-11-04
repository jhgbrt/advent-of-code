using System.Diagnostics;
using System.Text.RegularExpressions;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());


static class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    public static Result<int> Part1() => Run(1, () => Run().Count);
    public static Result<string> Part2() => Run(2, () => Run().ToString());

    static Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
    static Regex rect = new Regex("rect (?<rows>\\d*)x(?<cols>\\d)*", RegexOptions.Compiled);
    static Display Run()
    {
        var display = new Display(6, 50);
        foreach (var line in input)
        {
            var matchRect = rect.Match(line);
            if (matchRect.Success)
            {
                var rows = int.Parse(matchRect.Groups["rows"].ToString());
                var cols = int.Parse(matchRect.Groups["cols"].ToString());
                display.Rect(rows, cols);
            }
            var matchRotate = rotate.Match(line);
            if (matchRotate.Success)
            {

                var op = matchRotate.Groups["op"].ToString();
                var i = int.Parse(matchRotate.Groups["i"].ToString());
                var by = int.Parse(matchRotate.Groups["by"].ToString());
                if (op == "row")
                    display.RotateRow(i, by);
                else
                    display.RotateCol(i, by);
            }
        }
        return display;

    }

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
    public void Test1() => Assert.Equal(123, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(@"
.##..####.###..#..#.###..####.###....##.###...###.
#..#.#....#..#.#..#.#..#....#.#..#....#.#..#.#....
#..#.###..###..#..#.#..#...#..###.....#.#..#.#....
####.#....#..#.#..#.###...#...#..#....#.###...##..
#..#.#....#..#.#..#.#....#....#..#.#..#.#.......#.
#..#.#....###...##..#....####.###...##..#....###..
", Part2().Value);

    [Fact]
    public void GetRow()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

        Assert.Equal(new[] { 11, 12, 13, 14, 15 }, x.Row(0));
    }

    [Fact]
    public void GetColumn()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

        Assert.Equal(new[] { 12, 22, 32, 42 }, x.Column(1));
    }

    [Fact]
    public void RotateRight()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

        Assert.Equal(new[] { 14, 15, 11, 12, 13 }, x.Row(0).ToList().Rotate(2));
    }

    [Fact]
    public void RotateColumn()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
            };
        x.RotateCol(1, 1);
        Assert.Equal(new[,] {
                {11, 32, 13, 14, 15},
                {21, 12, 23, 24, 25},
                {31, 22, 33, 34, 35},
            }, x);
    }

    [Fact]
    public void ReplaceRow()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

        x.ReplaceRow(2, new[] { 1, 2, 3, 4, 5 });

        Assert.Equal(new[,] {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {1, 2, 3, 4, 5},
                {41, 42, 43, 44, 45}
            }, x);
    }
    [Fact]
    public void ReplaceCol()
    {
        int[,] x =
        {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

        x.ReplaceCol(3, new[] { 1, 2, 3, 4 });

        Assert.Equal(new[,] {
                {11, 12, 13, 1, 15},
                {21, 22, 23, 2, 25},
                {31, 32, 33, 3, 35},
                {41, 42, 43, 4, 45}
            }, x);
    }

    [Fact]
    public void RegexRotate()
    {
        Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
        var match = rotate.Match("rotate row y=7 by 20");
        Assert.Equal("7", match.Groups["i"].Value);
        Assert.Equal("20", match.Groups["by"].Value);
        Assert.Equal("row", match.Groups["op"].Value);
    }

}

readonly record struct Result<T>(T Value, TimeSpan Elapsed);


