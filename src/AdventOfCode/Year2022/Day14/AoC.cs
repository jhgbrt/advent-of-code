namespace AdventOfCode.Year2022.Day14;
public class AoC202214
{
    static string[] input = Read.InputLines();
    static Grid Parse(IEnumerable<string> input, int part)
    {
        var tiles = from l in input
                    from pair in (
                        from item in l.Split(" -> ")
                        let pair = item.Split(',')
                        let x = int.Parse(pair[0])
                        let y = int.Parse(pair[1])
                        select new Point(x, y)
                        ).Windowed2()
                    let line = Line.From(pair.a, pair.b)
                    from p in line.Points()
                    select p;

        return new Grid(tiles, part);
    }

    public int Part1() => Parse(input, 1).DoSimulation();
    public int Part2() => Parse(input, 2).DoSimulation();
}


class Grid
{
    private Dictionary<Point, char> tiles;

    private readonly int Floor;
    private readonly Point Source = new(500, 0);
    private readonly int part;

    public Grid(IEnumerable<Point> tiles, int part)
    {
        this.part = part;
        this.tiles = tiles.Distinct().ToDictionary(t => t, _ => '#');
        this[Source] = '+';
        this.Floor = tiles.Max(x => x.y) + part - 1;
    }

    public int DoSimulation()
    {
        int steps = 0;
        while (Step())
        {
            steps++;
        }
        return steps - 1 + part;
    }

    bool Step()
    {
        var position = Source;

        while (CanMove(position))
        {
            while (CanMoveDown(position))
                position = position.Down();
            if (CanMoveDownLeft(position))
                position = position.DownLeft();
            else if (CanMoveDownRight(position))
                position = position.DownRight();
        }

        this[position] = 'o';

        return part switch
        {
            1 => position.y < Floor,
            2 => position != Source,
            _ => false
        };
    }


    bool CanMove(Point pos) => CanMoveDown(pos) || CanMoveDownLeft(pos) || CanMoveDownRight(pos);
    bool CanMoveDown(Point pos) => pos.y < Floor && this[pos.Down()] == '.';
    bool CanMoveDownLeft(Point pos) => pos.y < Floor && this[pos.DownLeft()] == '.';
    bool CanMoveDownRight(Point pos) => pos.y < Floor && this[pos.DownRight()] == '.';
    char this[Point c]
    {
        get => tiles.TryGetValue(c, out char value) ? value : '.';
        set { tiles[c] = value; }
    }

 
    public override string ToString()
    {
    int minX = tiles.Keys.Min(x => x.x);
    int maxX = tiles.Keys.Max(x => x.x);
    var sb = new StringBuilder();
        for (int y = 0; y <= Floor; y++)
        {
            for (int x = minX; x <= maxX; x++)
                sb.Append(this[new(x, y)]);
            if (y != Floor)
                sb.AppendLine();
        }
        return sb.ToString();
    }
}
static class Ex
{
    public static Point Down(this Point c) => c with { y = c.y + 1 };
    public static Point DownLeft(this Point c) => c with { x = c.x - 1, y = c.y + 1 };
    public static Point DownRight(this Point c) => c with { x = c.x + 1, y = c.y + 1 };
    public static IEnumerable<(T a, T b)> Windowed2<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var b = enumerator.Current;
            yield return (a, b);
            a = b;
        }
    }
}
record Line(Point start, Point end)
{
    public static Line From(Point p1, Point p2) => (p2.y - p1.y, p2.x - p1.x) switch
    {
        ( > 0, 0) => new Line(p1, p2),
        (0, > 0) => new Line(p1, p2),
        ( < 0, 0) => new Line(p2, p1),
        (0, < 0) => new Line(p2, p1),
        _ => throw new NotSupportedException("expected straight line")
    };
    public IEnumerable<Point> Points() => start.x == end.x
            ? Range(start.y, end.y - start.y + 1).Select(y => new Point(start.x, y))
            : Range(start.x, end.x - start.x + 1).Select(x => new Point(x, start.y));
}

readonly record struct Point(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}
