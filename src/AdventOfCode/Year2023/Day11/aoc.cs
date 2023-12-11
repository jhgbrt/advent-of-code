namespace AdventOfCode.Year2023.Day11;
public class AoC202311
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] realinput = Read.InputLines();
    static string[] input = usesample ? sample : realinput;
    public object Part1()
    {
        return Solve(2);
    }

    private static object Solve(int n)
    {
        var grid = new Grid(input);

        var emptyrows = (from r in grid.Rows
                        where r.row.All(c => c == '.')
                        select r.y).Reverse().ToArray();
        var emptycolumns = (from c in grid.Columns
                           where c.column.All(c => c == '.')
                           select c.x).Reverse().ToArray();

        foreach (var y in emptyrows)
        {
            grid = grid.InsertRow(y, n);
        }
        foreach (var x in emptycolumns)
        {
            grid = grid.InsertColumn(x, n);
        }


        var points = (from item in grid.Points()
                      where grid[item] == '#'
                      select item).ToList();

        var pairs = from p1 in points from p2 in points select (p1, p2);



        var distances = from p1 in points
                        from p2 in points
                        select p1.ManhattanDistance(p2);



        return distances.Sum() / 2;
    }

    public object Part2() => Solve(usesample ? 10 : 1000000);
}


readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}


class Grid
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    public int Height => bottomright.y;
    public int Width => bottomright.x;
    public Grid(string[] input, char empty = '.')
    {
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 where input[y][x] != empty
                 select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        bottomright = new(input[0].Length, input.Length);
        this.empty = empty;
    }

    private Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate bottomright)
    {
        this.items = items;
        this.empty = empty;
        this.bottomright = bottomright;
    }

    public Grid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new Grid(builder.ToImmutable(), empty, bottomright);
    }
    public IEnumerable<(int x, IEnumerable<char> column)> Columns
    {
        get {
            for (int x = 0; x < bottomright.x; x++)
            {
                yield return (x, from y in Range(0, bottomright.y) select this[x, y]);
            }
        }
    }
    public IEnumerable<(int y, IEnumerable<char> row)> Rows
    {
        get {
            for (int y = 0; y < bottomright.y; y++)
            {
                yield return (y, from x in Range(0, bottomright.x) select this[x, y]);
            }
        }
    }
    public Grid InsertColumn(int x, int n = 2)
    {
        var b = items.ToBuilder();
        var offset = n - 1;
        var maxX = bottomright.x + offset;
        for (int y = 0; y < bottomright.y; y++)
        {
            for (int i = maxX - 1; i >= x + offset; i--)
            {
                if (this[i - offset, y] == empty)
                {
                    b.Remove(new(i, y), out char _);
                }
                else
                {
                    b[new(i, y)] = this[i - offset, y];
                }
            }
            b[new(x, y)] = empty;
        }
        return new Grid(b.ToImmutable(), empty, bottomright with { x = maxX });
    }
    public Grid InsertRow(int y, int n = 2)
    {
        var b = items.ToBuilder();
        var offset = n - 1;
        var maxY = bottomright.y + offset;

        for (int x = 0; x < bottomright.x; x++)
        {
            for (int i = maxY - 1; i >= y + offset; i--)
            {
                if (this[x, i - offset] == empty)
                {
                    b.Remove(new(x, i), out char _);
                }
                else
                {
                    b[new(x, i)] = this[x, i - offset];
                }
            }
            b[new(x, y)] = empty;
        }
        return new Grid(b.ToImmutable(), empty, bottomright with { y = maxY });
    }

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, bottomright.y)
        from x in Range(origin.x, bottomright.x)
        select new Coordinate(x, y);

    public IEnumerable<Coordinate> Neighbours(Coordinate p)
    {
        return
            from d in new (int x, int y)[]
            {
                (-1, -1),
                (-1, 0),
                (-1, 1),
                (0, 1),
                (1, 1),
                (1, 0),
                (1, -1),
                (0, -1)
            }
            where IsValid(p + d)
            select (p + d);
    }

    Coordinate? IfValid(Coordinate p) => IsValid(p) ? p : null;
    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < bottomright.x && p.y < bottomright.y;

    public IEnumerable<Coordinate> BoundingBox(Coordinate p, int length)
    {
        return
            from x in Range(p.x - 1, length + 2)
            from y in new[] { p.y - 1, p.y, p.y + 1 }
            where x >= 0 && y >= 0
            && x < bottomright.x
            && y < bottomright.y
            select new Coordinate(x, y);
    }

    public IEnumerable<Coordinate> InteriorPoints() =>
        from y in Range(origin.y + 1, bottomright.y - 2)
        from x in Range(origin.x + 1, bottomright.x - 2)
        select new Coordinate(x, y);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < bottomright.y; y++)
        {
            for (int x = origin.x; x < bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public bool Contains(Coordinate c) => items.ContainsKey(c);
}
