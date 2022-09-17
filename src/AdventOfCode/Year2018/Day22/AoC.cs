namespace AdventOfCode.Year2018.Day22;

public class AoC201822
{
    const int depth = 5616;
    const int targetx = 10;
    const int targety = 785;

    public object Part1() => Points().Select(p => RiskLevel(p)).Sum();
    public object Part2()
    {
        return string.Empty;
    }

    IEnumerable<Point> Points()
    {
        for (int x = 0; x <= targetx; x++)
            for (int y = 0; y <= targety; y++)
                yield return new Point(x, y);
    }

    int RiskLevel(Point p) => ErosionLevel(p) % 3;

    int GeologicIndex(Point p) => p switch
    {
        (0, 0) => 0,
        (targetx, targety) => 0,
        (_, 0) => p.x * 16807,
        (0, _) => p.y * 48271,
        _ => ErosionLevel(new(p.x - 1, p.y)) * ErosionLevel(new(p.x, p.y - 1))
    };

    Dictionary<Point, int> _cache = new();
    private int ErosionLevel(Point p)
    {
        if (_cache.ContainsKey(p)) return _cache[p];
        var result = (GeologicIndex(p) + depth) % 20183;
        _cache[p] = result;
        return result;
    }
}

readonly record struct Point(int x, int y)
{
    internal IEnumerable<Point> Neighbours()
    {
        if (x > 0) yield return new Point(x - 1, y);
        yield return new Point(x, y + 1);
        if (y > 0) yield return new Point(x, y - 1);
        yield return new Point(x + 1, y);
    }
}