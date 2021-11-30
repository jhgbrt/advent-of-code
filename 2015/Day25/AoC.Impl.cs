namespace AdventOfCode.Year2015.Day25;

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");
    const int row = 3010;
    const int column = 3019;
    const long code = 20151125;
    const long m = 252533;
    const long d = 33554393;

    internal static Result Part1() => Run(() =>
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
    internal static Result Part2() => Run(() => -1);

}