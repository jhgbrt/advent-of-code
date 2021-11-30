namespace AdventOfCode.Year2015.Day25;

public class AoCImpl : AoCBase
{
    
    public static string[] input = Read.InputLines(typeof(AoCImpl));
    const int row = 3010;
    const int column = 3019;
    const long code = 20151125;
    const long m = 252533;
    const long d = 33554393;

    public override object Part1()
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
    }
    public override object Part2() => -1;

}