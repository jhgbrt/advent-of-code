namespace AdventOfCode.Common;


class FiniteGrid
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
    public FiniteGrid(string[] input, char empty = '.')
    {
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 where input[y][x] != empty
                 select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        bottomright = new(input[0].Length, input.Length);
        this.empty = empty;
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
            from d in new (int x, int y)[] { (-1, 0), (0, 1), (1, 0), (0, -1) }
            where p.x + d.x >= 0
            && p.y + d.y >= 0
            && p.x + d.x < bottomright.x
            && p.y + d.y < bottomright.y
            select new Coordinate(p.x + d.x, p.y + d.y);
    }
    public IEnumerable<Coordinate> BoundingBox(Coordinate p, int length)
    {
        return
            from x in Range(p.x - 1, length + 2)
            from y in new[]{p.y - 1, p.y, p.y + 1}
            where x >= 0 && y >= 0
            && x < bottomright.x
            && y < bottomright.y
            select new Coordinate(x, y);
    }

    private IEnumerable<(Coordinate, char)> Sequence(int x, int y, Func<char,bool> predicate)
    {
        {
            yield return (new(x, y), this[x, y]);
            x++;
        }

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
}


class InfiniteGrid
{
    readonly Dictionary<Coordinate, char> items = new();
    readonly char empty = '.';
    public char this[Coordinate p]
    {
        get
        {
            return items.TryGetValue(p, out var c) ? c : empty;
        }
        set
        {
            if (value == empty)
            {
                items.Remove(p);
            }
            else
            {
                items[p] = value;
            }
        }
    }

    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    private Coordinate topleft => new(items.Keys.Min(x => x.x), items.Keys.Min(x => x.y));
    private Coordinate bottomright => new(items.Keys.Max(x => x.x), items.Keys.Max(x => x.y));

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = topleft.y; y <= bottomright.y; y++)
        {
            for (int x = topleft.x; x <= bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
