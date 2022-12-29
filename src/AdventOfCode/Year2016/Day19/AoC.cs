namespace AdventOfCode.Year2016.Day19;

public class AoC201619
{
    int input = 3001330;

    public object Part1()
    {
        int n = input;
        var p = 1;
        while (p < n)
        {
            p *= 2;
        }
        if (p == n) return 1;
        return 2 * n - p + 1;
    }

    public object Part2()
    {
        int n = input;
        int p = 1;
        while (p*3 < n)
            p *= 3;
        return n - p;
    }
}