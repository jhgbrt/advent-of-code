namespace AdventOfCode.Year2023.Day13;
public class AoC202313
{
    public AoC202313() : this(Console.Out, Read.InputLines()) { }

    readonly ImmutableArray<Grid> grids;
    readonly TextWriter writer;
    internal IEnumerable<Grid> Items => grids;
    public AoC202313(TextWriter writer, string[] input)
    {
        grids = Parse(input).ToImmutableArray();
        this.writer = writer;
    }

    IEnumerable<Grid> Parse(IEnumerable<string> input)
    {
        var e = input.GetEnumerator();
        while (true)
        {
            var lines = e.While(s => !string.IsNullOrEmpty(s)).ToArray();
            if (lines.Length == 0) yield break;
            yield return new Grid(lines);
        }
    }

    public int Part1() => (from grid in grids
                           let rotated = grid.Rotate90()
                           select FindMirror(rotated, 0) ?? FindMirror(grid, 0) * 100 ?? 0
                            ).Sum();
    public int Part2() => (from grid in grids
                           let rotated = grid.Rotate90()
                           select FindMirror(rotated, 1) ?? FindMirror(grid, 1) * 100 ?? 0
                            ).Sum();
    int? FindMirror(Grid grid, int smudges = 0) 
        => (from y in Range(0, grid.Height - 1)
            let count = Min(y + 1, grid.Height - y - 1)
            let diffs = (
                from offset in Range(0, count)
                select grid.Columns.Count(x => grid[x, y - offset] != grid[x, y + offset + 1])
                ).Sum()
            where diffs == smudges
            select (int?)(y + 1)).FirstOrDefault();
}



class Grid
{
    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    public int Height => bottomright.y;
    public int Width => bottomright.x;
    public Grid(string[] input, char empty = '.') 
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {}

    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    private Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate bottomright)
    {
        this.items = items;
        this.empty = empty;
        this.bottomright = bottomright;
    }

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];
    public IEnumerable<int> Rows => Range(0, Height);
    public IEnumerable<int> Columns => Range(0, Width);
    public Grid Rotate90() => Transform(p => (Height - p.y - 1, p.x));
    public Grid Transform(Func<(int x, int y), (int x, int y)> transform)
    {
        var q = (
            from x in Range(0, Width)
            from y in Range(0, Height)
            where items.ContainsKey(new(x, y))
            let transformed = transform((x,y))
            select (transformed.x, transformed.y, c: items[new(x, y)])
            ).ToImmutableDictionary(v => new Coordinate(v.x, v.y), v => v.c);

        return new(q, empty, new(q.Keys.Max(k => k.x)+1, q.Keys.Max(k => k.y)+1));
    }

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

    public bool Contains(Coordinate c) => c.x >= 0 && c.x < bottomright.x && c.y >= 0 && c.y < bottomright.y;

}

readonly record struct Coordinate(int x, int y);

public class AoC202313Tests
{
    private TextWriter output;
    public AoC202313Tests(ITestOutputHelper helper)
    {
        output = new TestWriter(helper);
    }

    [Fact]
    public void TestParsing()
    {
        var input = Read.Sample(1).Lines().ToArray();
        AoC202313 sut = new AoC202313(output, input);
        Assert.Equal(2, sut.Items.Count());
    }

    [Theory]
    [InlineData(1, 405)]
    [InlineData(2, 709)]
    public void TestPart1(int sample, int expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        AoC202313 sut = new AoC202313(output, input);
        Assert.Equal(expected, sut.Part1());
    }

    [Theory]
    [InlineData(1, 400)]
    [InlineData(2, 1400)]
    public void TestPart2(int sample, int expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        AoC202313 sut = new AoC202313(output, input);
        Assert.Equal(expected, sut.Part2());
    }

}