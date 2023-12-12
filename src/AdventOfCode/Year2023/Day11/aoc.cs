namespace AdventOfCode.Year2023.Day11;
public class AoC202311
{
    static string[] input = Read.InputLines();
    Grid grid;
    public AoC202311() : this(input)
    {
    }
    internal AoC202311(string[] input)
    {
        grid = new Grid(input);
    }
    public object Part1() => Solve(2);
    public object Part2() => Solve(1000000);

    internal long Solve(int n)
    {
        var emptyrows =
            (from r in grid.Rows
             where r.row.All(c => c == '.')
             select r.y).Reverse().ToArray();
        var emptycols =
            (from c in grid.Columns
             where c.column.All(c => c == '.')
             select c.x).Reverse().ToArray();

        var points =
            (from item in grid.Points()
             where grid[item] == '#'
             select item).ToList();

        var distances =
            from p1 in points
            from p2 in points
            let ranges = (x: (p1.x, p2.x), y: (p1.y, p2.y))
            let emptyr = emptyrows.Count(y => ranges.y.Contains(y))
            let emptyc = emptycols.Count(x => ranges.x.Contains(x))
            select p1.ManhattanDistance(p2) + emptyr * (n - 1L) + emptyc * (n - 1L);

        return distances.Sum() / 2;
    }

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

    public IEnumerable<(int x, IEnumerable<char> column)> Columns
    {
        get 
        {
            for (int x = 0; x < bottomright.x; x++)
            {
                yield return (x, from y in Range(0, bottomright.y) select this[x, y]);
            }
        }
    }
    public IEnumerable<(int y, IEnumerable<char> row)> Rows
    {
        get 
        {
            for (int y = 0; y < bottomright.y; y++)
            {
                yield return (y, from x in Range(0, bottomright.x) select this[x, y]);
            }
        }
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

    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < bottomright.x && p.y < bottomright.y;

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



public class Tests
{
    [Theory]
    [InlineData(2, 374)]
    [InlineData(10, 1030)]
    [InlineData(100, 8410)]
    public void TestAoC202311(int n, int expected)
    {
        var input = Read.SampleLines();
        var sut = new AoC202311(input);
        Assert.Equal(expected, sut.Solve(n));
    }

}