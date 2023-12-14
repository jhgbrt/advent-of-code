namespace AdventOfCode.Year2023.Day14;
public class AoC202314
{
    public AoC202314():this(Read.InputLines(), Console.Out) {}

    readonly Grid grid;
    readonly TextWriter writer;
    public AoC202314(string[] input, TextWriter writer)
    {
        grid = new Grid(input);
        this.writer = writer;
    }

    public object Part1()
    {
        var grid = this.grid;
        foreach (var p in grid.Points())
        {
            if (grid[p] == 'O')
            {
                int y = p.y;
                while (y > 0 && grid[p.x, y - 1] == '.') y--;
                if (y >= 0 && grid[p.x, y] == '.') 
                {
                    grid = grid.With(g => { g[p] = '.'; g[new(p.x, y)] = 'O'; });
                }
            }
        }
        var weight = grid.Height;
        var total = 0;
        for (int y = 0; y < grid.Height; y++)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                if (grid[x, y] == 'O') total += weight;
            }
            weight--;
        }

        return total;
    }
    public object Part2() => "";
}

enum Direction { N, NE, E, SE, S, SW, W, NW }
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
    public Coordinate Find(char c) => items.Where(i => i.Value == c).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points()
    {
        for (int x = 0; x < Width; x++)
        for (int y = 0; y < Height; y++) yield return new Coordinate(x, y);
    }


    public IEnumerable<(Direction d, Coordinate c)> Neighbours(Coordinate p)
    {
        return
            from d in new (Direction direction, (int x, int y) delta)[]
            {
                (Direction.NW, (-1, -1)),
                (Direction.W, (-1, 0)),
                (Direction.SW, (-1, 1)),
                (Direction.S, (0, 1)),
                (Direction.SE, (1, 1)),
                (Direction.E, (1, 0)),
                (Direction.NE, (1, -1)),
                (Direction.N, (0, -1))
            }
            where IsValid(p + d.delta)
            select (d.direction, p + d.delta);
    }

    public Coordinate? GetNeighbour(Coordinate p, Direction d) => d switch
    {
        Direction.N => IfValid(new(p.x, p.y - 1)),
        Direction.NE => IfValid(new(p.x + 1, p.y - 1)),
        Direction.E => IfValid(new(p.x + 1, p.y)),
        Direction.SE => IfValid(new(p.x + 1, p.y + 1)),
        Direction.S => IfValid(new(p.x, p.y + 1)),
        Direction.SW => IfValid(new(p.x - 1, p.y + 1)),
        Direction.W => IfValid(new(p.x - 1, p.y)),
        Direction.NW => IfValid(new(p.x - 1, p.y - 1))
    };
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

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";

    public static Slope operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
    public double Angle(Coordinate other) => -Atan2(x - other.x, y - other.y);
    public Coordinate N => new(x, y - 1);
    public Coordinate NE => new(x + 1, y - 1);
    public Coordinate E => new(x + 1, y);
    public Coordinate SE => new(x + 1, y + 1);
    public Coordinate S => new(x, y + 1);
    public Coordinate SW => new(x - 1, y + 1);
    public Coordinate W => new(x - 1, y);
    public Coordinate NW => new(x - 1, y - 1);
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

public class AoC202314Tests
{
    private readonly AoC202314 sut;
    public AoC202314Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202314(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(136, sut.Part1());
    }

    [Fact]
    public void TestShift()
    {

    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}