using Sprache;

using System.Collections;

namespace AdventOfCode.Year2019.Day17;
public class AoC201917
{
    public AoC201917() : this(IntCodeToGrid(Read.InputLines()), Console.Out) { }
    readonly TextWriter writer;
    FiniteGrid grid;
    internal static string[] IntCodeToGrid(string[] input)
    {
        var program = input.First().Split(',').Select(long.Parse).ToArray();
        var intcode = new IntCode(program);
        var sb = new StringBuilder();
        var r = intcode.Run().Select(i => (char)i);
        foreach (var c in r) sb.Append(c);
        return Read.String(sb.ToString());
    }
    public AoC201917(string[] input, TextWriter writer)
    {
        grid = new FiniteGrid(input);
        this.writer = writer;
    }

    public int Part1() => (from c in grid.Points()
                           let n = grid.Neighbours(c)
                           where grid[c] == '#'
                           && n.All(n => grid[n] == '#')
                           select c.x * c.y).Sum();
    public int Part2() => -1;
}

public class AoC201917Tests
{
    private readonly AoC201917 sut;
    private readonly ITestOutputHelper output;

    public AoC201917Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC201917(input, new TestWriter(output));
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(76, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }
  

}



enum Direction { N, E, S, W }


class FiniteGrid
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.y;
    public int Width => endmarker.x;
    public FiniteGrid(string[] input, char empty = '.')
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {
    }
    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    internal FiniteGrid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }
    public FiniteGrid Rotate90() => Transform(p => (Height - p.y - 1, p.x));
    public FiniteGrid Rotate180() => Transform(p => (Width - p.x - 1, Height - p.y - 1));
    public FiniteGrid Rotate270() => Transform(p => (p.y, Width - p.x - 1));
    public FiniteGrid Transform(Func<(int x, int y), (int x, int y)> transform)
    {
        var q = (
            from x in Range(0, Width)
            from y in Range(0, Height)
            where items.ContainsKey(new(x, y))
            let transformed = transform((x, y))
            select (transformed.x, transformed.y, c: items[new(x, y)])
            ).ToImmutableDictionary(v => new Coordinate(v.x, v.y), v => v.c);

        return new(q, empty, new(q.Keys.Max(k => k.x) + 1, q.Keys.Max(k => k.y) + 1));
    }

    public FiniteGrid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new FiniteGrid(builder.ToImmutable(), empty, endmarker);
    }
    public Coordinate Find(char c) => items.Where(i => i.Value == c).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, endmarker.y)
        from x in Range(origin.x, endmarker.x)
        select new Coordinate(x, y);


    public IEnumerable<(Coordinate coordinate, char value)> this[int? x, int? y]
    {
        get {
            if (x.HasValue && y.HasValue)
            {
                yield return (new Coordinate(x.Value, y.Value), this[x.Value, y.Value]);
            }
            else if (x.HasValue)
            {
                foreach (var y1 in Range(0, Height))
                {
                    yield return (new Coordinate(x.Value, y1), this[x.Value, y1]);
                }
            }
            else if (y.HasValue)
            {
                foreach (var x1 in Range(0, Width))
                {
                    yield return (new Coordinate(x1, y.Value), this[x1, y.Value]);
                }
            }
            else
            {
                foreach (var y1 in Range(0, Height))
                {
                    foreach (var x1 in Range(0, Width))
                    {
                        yield return (new Coordinate(x1, y1), this[x1, y1]);
                    }
                }
            }
        }
    }

    public IEnumerable<Coordinate> Neighbours(Coordinate p)
    {
        return
            from d in new (Direction direction, (int x, int y) delta)[]
            {
                (Direction.W, (-1, 0)),
                (Direction.S, (0, 1)),
                (Direction.E, (1, 0)),
                (Direction.N, (0, -1))
            }
            where IsValid(p + d.delta)
            select p + d.delta;
    }

    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < endmarker.x && p.y < endmarker.y;

    public IEnumerable<Coordinate> BoundingBox(Coordinate p, int length)
    {
        return
            from x in Range(p.x - 1, length + 2)
            from y in new[] { p.y - 1, p.y, p.y + 1 }
            where x >= 0 && y >= 0
            && x < endmarker.x
            && y < endmarker.y
            select new Coordinate(x, y);
    }

    public IEnumerable<Coordinate> InteriorPoints() =>
        from y in Range(origin.y + 1, endmarker.y - 2)
        from x in Range(origin.x + 1, endmarker.x - 2)
        select new Coordinate(x, y);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < endmarker.y; y++)
        {
            for (int x = origin.x; x < endmarker.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public bool Contains(Coordinate c) => IsValid(c);

}

