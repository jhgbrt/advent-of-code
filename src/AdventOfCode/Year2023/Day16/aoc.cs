namespace AdventOfCode.Year2023.Day16;
public class AoC202316
{
    public AoC202316():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly Grid grid;
    public AoC202316(string[] input, TextWriter writer)
    {
        grid = new Grid(input);
        this.writer = writer;
    }

    public int Part1() => Calculate(new (new(-1,0), Direction.E));

    public object Part2() => (
                from x in Range(0, grid.Width)
                from p in ((y: -1, d: Direction.S), (y: grid.Height, d: Direction.N)).AsEnumerable()
                select new Vector(new(x, p.y), p.d)
                ).Concat(
                from y in Range(0, grid.Height)
                from p in ((x: -1, d: Direction.E), (x: grid.Width, d: Direction.W)).AsEnumerable()
                select new Vector(new(p.x, y), p.d)
            ).Select(Calculate).Max();


    int Calculate(Vector v) => FindEnergizedSpots(grid, v, []).Select(v => v.pos).Distinct().Count() - 1;

    IEnumerable<Vector> FindEnergizedSpots(Grid grid, Vector v, HashSet<Vector> seen)
    {
        if (seen.Contains(v)) yield break;
        yield return v;
        seen.Add(v);

        foreach (var w in v.Advance(grid))
        {
            foreach (var item in FindEnergizedSpots(grid, w, seen))
            {
                yield return item;
            }
        }

    }

}

public class AoC202316Tests
{
    private readonly AoC202316 sut;
    ITestOutputHelper output;
    public AoC202316Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202316(input, new TestWriter(output));
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(46, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(51, sut.Part2());
    }

    [Theory]
    [InlineData(Direction.N, Direction.E, Direction.W)]
    [InlineData(Direction.S, Direction.E, Direction.W)]
    [InlineData(Direction.E, Direction.N, Direction.S)]
    [InlineData(Direction.W, Direction.N, Direction.S)]
    public void Split(Direction d, params Direction[] expected)
    {
        Assert.Equal(expected, Vector.Split(d));
    }

    [Theory]
    [InlineData(1, 0, Direction.N, null, null)]
    [InlineData(1, 1, Direction.N, 1, 0)]
    [InlineData(1, 2, Direction.S, null, null)]
    [InlineData(1, 1, Direction.S, 1, 2)]
    [InlineData(2, 1, Direction.E, null, null)]
    [InlineData(1, 1, Direction.E, 2, 1)]
    [InlineData(0, 1, Direction.W, null, null)]
    [InlineData(1, 1, Direction.W, 0, 1)]
    public void Go(int x, int y, Direction d, int? x1, int? y1)
    {
        var grid = new Grid(new[] { "...", "...", "..." });
        var result = grid.Go(new(x, y), d);
        Coordinate? c = x1.HasValue && y1.HasValue ? new Coordinate(x1.Value, y1.Value) : null;
        Assert.Equal(c, result);
    }
    [Theory]
    [InlineData(@"./", 0, 0, Direction.E, 1, 0, Direction.N, null, null, null)]
    [InlineData(@"/.", 1, 0, Direction.W, 0, 0, Direction.S, null, null, null)]
    [InlineData(@".\", 0, 0, Direction.E, 1, 0, Direction.S, null, null, null)]
    [InlineData(@"\.", 1, 0, Direction.W, 0, 0, Direction.N, null, null, null)]
    [InlineData(@"..", 0, 0, Direction.E, 1, 0, Direction.E, null, null, null)]
    [InlineData(@"..", 1, 0, Direction.W, 0, 0, Direction.W, null, null, null)]
    [InlineData(@".-", 0, 0, Direction.E, 1, 0, Direction.E, null, null, null)]
    [InlineData(@"-.", 1, 0, Direction.W, 0, 0, Direction.W, null, null, null)]
    [InlineData(@".|", 0, 0, Direction.E, 1, 0, Direction.N, 1, 0, Direction.S)]
    [InlineData(@"|.", 1, 0, Direction.W, 0, 0, Direction.N, 0, 0, Direction.S)]
    [InlineData(".\n/", 0, 0, Direction.S, 0, 1, Direction.W, null, null, null)]
    [InlineData("/\n.", 0, 1, Direction.N, 0, 0, Direction.E, null, null, null)]
    [InlineData(".\n\\", 0, 0, Direction.S, 0, 1, Direction.E, null, null, null)]
    [InlineData("\\\n.", 0, 1, Direction.N, 0, 0, Direction.W, null, null, null)]
    [InlineData(".\n.", 0, 0, Direction.S, 0, 1, Direction.S, null, null, null)]
    [InlineData(".\n.", 0, 1, Direction.N, 0, 0, Direction.N, null, null, null)]
    [InlineData(".\n|", 0, 0, Direction.S, 0, 1, Direction.S, null, null, null)]
    [InlineData("|\n.", 0, 1, Direction.N, 0, 0, Direction.N, null, null, null)]
    [InlineData(".\n-", 0, 0, Direction.S, 0, 1, Direction.E, 0, 1, Direction.W)]
    [InlineData("-\n.", 0, 1, Direction.N, 0, 0, Direction.E, 0, 0, Direction.W)]
    public void Advance(string input, int x, int y, Direction d, int? x1, int? y1, Direction? d1, int? x2, int? y2, Direction? d2)
    {
        var vector = new Vector(new(x,y), d);
        var grid = new Grid(Read.String(input));
        output.WriteLine(grid.ToString());
        var result = vector.Advance(grid).ToArray();
        Vector? v1 = x1.HasValue && y1.HasValue && d1.HasValue ? new Vector(new(x1.Value, y1.Value), d1.Value) : null;
        Vector? v2 = x2.HasValue && y2.HasValue && d2.HasValue ? new Vector(new(x2.Value, y2.Value), d2.Value) : null;
        var vectors = new[]{ v1, v2 }.Where(v => v.HasValue).Select(v => v!.Value).ToArray();
        Assert.Equal(vectors, result);
    }
}


public enum Direction { N, E, S, W }
class Grid
{

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

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[int x, int y] => this[new Coordinate(x, y)];

    Coordinate? IfValid(Coordinate p) => IsValid(p) ? p : null;
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
    public Coordinate? Go(Coordinate c, Direction d) 
    => d switch 
    { 
        Direction.N => IfValid(c.N), 
        Direction.E => IfValid(c.E), 
        Direction.S => IfValid(c.S), 
        Direction.W => IfValid(c.W) 
    };
}


readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";
    public Coordinate N => new(x, y - 1);
    public Coordinate E => new(x + 1, y);
    public Coordinate S => new(x, y + 1);
    public Coordinate W => new(x - 1, y);
}

readonly record struct Vector(Coordinate pos, Direction d)
{
    public override string ToString() => $"{pos},{d}";
    internal IEnumerable<Vector> Advance(Grid grid)
    {
        var go = grid.Go(pos, d);
        if (go is null) return Empty<Vector>();
        var next = go.Value;
        return (grid[next], d) switch
        {
            ('.', _) or ('-', Direction.E or Direction.W) or ('|', Direction.N or Direction.S) => new Vector(next, d).AsEnumerable(),
            ('-', Direction.N or Direction.S) or ('|', Direction.E or Direction.W) => Split(d).Select(x => new Vector(next, x)),
            ('/', Direction.N) => new Vector(next, Direction.E).AsEnumerable(),
            ('/', Direction.E) => new Vector(next, Direction.N).AsEnumerable(),
            ('/', Direction.S) => new Vector(next, Direction.W).AsEnumerable(),
            ('/', Direction.W) => new Vector(next, Direction.S).AsEnumerable(),
            ('\\', Direction.N) => new Vector(next, Direction.W).AsEnumerable(),
            ('\\', Direction.W) => new Vector(next, Direction.N).AsEnumerable(),
            ('\\', Direction.E) => new Vector(next, Direction.S).AsEnumerable(),
            ('\\', Direction.S) => new Vector(next, Direction.E).AsEnumerable()
        };
    }
    internal static IEnumerable<Direction> Split(Direction d) => d switch
    {
        Direction.N or Direction.S => [Direction.E, Direction.W],
        Direction.E or Direction.W => [Direction.N, Direction.S]
    };

}
