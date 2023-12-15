namespace AdventOfCode.Year2023.Day14;
public class AoC202314
{
    public AoC202314() : this(Read.InputLines()) { }

    readonly Grid grid;
    public AoC202314(string[] input)
    {
        grid = new Grid(input, '.');
    }

    public object Part1() => Weight(grid.Tilt(Direction.N));

    private static int Weight(Grid grid) => (from c in grid.FindAll('O')
                                             select grid.Height - c.y).Sum();

    public int Part2()
    {
        var weights = new List<int>();
        var seen = new Dictionary<string, int>();
        var grid = this.grid;

        const int cycles = 1000000000;

        for (var i = 0; i < cycles; i++)
        {
            grid = grid.Spin();
            var key = grid.ToString();
            if (seen.ContainsKey(key))
            {
                var g = seen[key];
                var index = (cycles - i - 1) % (i - g) + g;
                return weights[index];
            }
            seen[key] = i;
            weights.Add(Weight(grid));
        }
        return -1;
    }
}

enum Direction { N, E, S, W }
class Grid
{

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    public int Height => bottomright.y;
    public int Width => bottomright.x;

    public Grid(string[] input, char empty)
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

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[int x, int y] => this[new Coordinate(x, y)];
    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < bottomright.x && p.y < bottomright.y;
    public IEnumerable<Coordinate> FindAll(char value) => items.Keys.Where(k => items[k] == value);
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


    internal Grid Spin()
    {
        var b = items.ToBuilder();
        Tilt(b, Direction.N);
        Tilt(b, Direction.W);
        Tilt(b, Direction.S);
        Tilt(b, Direction.E);
        return new Grid(b.ToImmutable(), empty, bottomright);
    }
    internal Grid Tilt(Direction d)
    {
        var b = items.ToBuilder();
        Tilt(b, d);
        return new Grid(b.ToImmutable(), empty, bottomright);
    }

    private void Tilt(IDictionary<Coordinate, char> b, Direction d)
    {
        foreach (var p in b.Keys.Where(p => b[p] == 'O').OrderFor(d).ToList())
        {
            var next = FindLast(b, p, d);
            if (!b.ContainsKey(next))
            {
                b.Remove(p);
                b[next] = 'O';
            }
        }
    }

    private Coordinate FindLast(IDictionary<Coordinate,char> grid, Coordinate p, Direction d)
    {
        var current = p;
        var next = current.Shift(d);
        while (IsValid(next) && !grid.ContainsKey(next))
        {
            current = next;
            next = next.Shift(d);

        }
        return current;
    }
}

readonly record struct Coordinate(int x, int y)
{
    internal Coordinate Shift(Direction n) => n switch
    {
        Direction.N => new(x, y - 1),
        Direction.E => new(x + 1, y),
        Direction.S => new(x, y + 1),
        Direction.W => new(x - 1, y)

    };
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

static class CoordinateExtensions
{
    public static IEnumerable<Coordinate> OrderFor(this IEnumerable<Coordinate> list, Direction d) => d switch
    {
        Direction.N => from p in list orderby p.x, p.y select p,
        Direction.W => from p in list orderby p.y, p.x select p,
        Direction.S => from p in list orderby -p.x, -p.y select p,
        Direction.E => from p in list orderby -p.y, -p.x select p,

    };
}

public class AoC202314Tests
{
    private readonly AoC202314 sut;
    ITestOutputHelper output;
    public AoC202314Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202314(input);
        this.output = output;
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(136, sut.Part1());
    }


    [Fact]
    public void TestPart2()
    {
        Assert.Equal(64, sut.Part2());
    }
}

