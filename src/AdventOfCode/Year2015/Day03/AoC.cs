namespace AdventOfCode.Year2015.Day03;

public class AoC201503
{
    static string input = Read.InputText();
    public object Part1() => Points1(input).Distinct().Count();
    public object Part2() => Points2(input, 0).Concat(Points2(input, 1)).Distinct().Count();
    static IEnumerable<Point> Points1(string s)
    {
        var p = new Point(0, 0);

        yield return p;

        foreach (var c in s)
        {
            p = p.Next(c);
            yield return p;
        }
    }
    static IEnumerable<Point> Points2(string s, int start)
    {
        var p = new Point(0, 0);

        yield return p;

        for (int i = start; i < s.Length; i += 2)
        {
            p = p.Next(s[i]);
            yield return p;
        }
    }
}
record struct Point(int x, int y)
{
    public Point Next(char c) => c switch
    {
        '<' => this with { x = x - 1 },
        '>' => this with { x = x + 1 },
        '^' => this with { y = y - 1 },
        'v' => this with { y = y + 1 },
        _ => throw new Exception()
    };
}
