using Spectre.Console;

namespace AdventOfCode.Year2023.Day13;
public class AoC202313
{
    public AoC202313() : this(Console.Out, Read.InputLines()) { }

    readonly ImmutableArray<Grid> items;
    readonly TextWriter writer;
    internal IEnumerable<Grid> Items => items;
    public AoC202313(TextWriter writer, string[] input)
    {
        items = Parse(input).ToImmutableArray();
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

    public object Part1()
    {

        return (from grid in items
                let mirror = grid.FindMirror()
                select mirror.x.HasValue ? mirror.x.Value
                : mirror.y!.Value * 100).Sum();
    }
    public object Part2()
    {
    var q = (from grid in items
            let mirror = (
                from p in grid.Points()
                let flipped = grid.Flip(p)
                let mirror = flipped.FindMirror()
                where mirror.x.HasValue || mirror.y.HasValue
                select mirror).First()
            select mirror.x.HasValue ? mirror.x.Value
                : mirror.y!.Value * 100
                ).Sum();
        return q;
    }
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
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public (int? x, int? y) FindMirror()
    {
        foreach (var col in Range(0, Width))
        {
            if (IsVerticalSymmetryAxis(col)) return (col, null);
        }
        foreach (var row in Range(0, Height))
        {
            if (IsHorizontalSymmetryAxis(row)) return (null, row);
        }

        return (null, null);
    }

    public Grid Flip(Coordinate p)
    {
        var b = items.ToBuilder();
        if (b.ContainsKey(p)) b.Remove(p);
        else b[p] = '#';
        return new(b.ToImmutable(), empty, bottomright);
    }

    internal bool IsVerticalSymmetryAxis(int axis)
    {
        if (axis < 1) return false;
        if (axis >= Width) return false;
        foreach (var y in Range(0, Height))
        {
            if (!IsSymmetricRow(axis, y)) return false;
        }
        return true;
    }
    internal bool IsHorizontalSymmetryAxis(int axis)
    {
        if (axis < 1) return false;
        if (axis >= Height) return false;
        foreach (var x in Range(0, Width))
        {
            if (!IsSymmetricColumn(axis, x)) return false;
        }
        return true;
    }

    bool IsSymmetricRow(int axis, int y)
    {
        for (int offset = 0; axis - offset - 1 >= 0 && axis + offset < Width; offset++)
        {
            if (this[axis - offset - 1, y] != this[axis + offset, y]) return false;
        }
        return true;
    }

    bool IsSymmetricColumn(int axis, int x)
    {
        for (int offset = 0; axis - offset - 1 >= 0 && axis + offset < Height; offset++)
        {
            if (this[x, axis - offset - 1] != this[x, axis + offset]) return false;
        }
        return true;
    }

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, bottomright.y)
        from x in Range(origin.x, bottomright.x)
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

    public bool Contains(Coordinate c) => c.x >= 0 && c.x < bottomright.x && c.y >= 0 && c.y < bottomright.y;

}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

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
    [InlineData(2, 709)]
    public void TestPart2(int sample, int expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        AoC202313 sut = new AoC202313(output, input);
        Assert.Equal(expected, sut.Part2());
    }

    [Fact]
    public void IsVerticalSymmetryAxisTest()
    {
        var grid = new Grid(Read.String("""
        ##.
        ..#
        """));
        Assert.True(grid.IsVerticalSymmetryAxis(1));
        Assert.False(grid.IsVerticalSymmetryAxis(0));
        Assert.False(grid.IsVerticalSymmetryAxis(2));
    }

    [Fact]
    public void IsHorizontalSymmetryAxisTest()
    {
        var grid = new Grid(Read.String("""
        #.
        #.
        .#
        """));
        Assert.True(grid.IsHorizontalSymmetryAxis(1));
        Assert.False(grid.IsHorizontalSymmetryAxis(0));
        Assert.False(grid.IsHorizontalSymmetryAxis(2));
    }

    [Theory]
    [InlineData("""
    #.###..#..###
    .#...##.####.
    .#...##.####.
    #.###..#..###
    .#######.##.#
    .#..##.#.#..#
    """, null, 2)]
    [InlineData("""
    .#...##.####.
    .#...##.####.
    .#######.##.#
    .#..##.#.#..#
    """, null, 1)]
    [InlineData("""
    .#######.##.#
    .#..##.#.#..#
    .#...##.####.
    .#...##.####.
    """, null, 3)]
    [InlineData("""
    #..#..
    #..#.#
    .##...
    """, 2, null)]
    [InlineData("""
    ##..#..
    ##..#..
    ###...#
    """, 1, null)]
    [InlineData("""
    ##
    ..
    ##
    """, 1, null)]
    [InlineData("""
    ..##
    ..##
    .###
    """, 3, null)]
    [InlineData("""
    ..####
    ..####
    .#####
    """, 4, null)]

    [InlineData("""
    ##..#..#..##.#.##
    .###....###.###.#
    .#.##..##.#....#.
    ####.##..###.####
    .###....###.....#
    .#.#....#.#..#...
    .##.####.##.##...
    .##.####.##.##...
    """, null, 7)]
    [InlineData("""
    ###.##..##.
    .#.###..###
    ###...##...
    ......##...
    .##.##..##.
    ...#..##..#
    #.#.######.
    #.#.#.##.#.
    ..###....##
    ###..#####.
    .##.#.##.#.
    .##########
    .#.........
    ###...##...
    ##..#.##.#.
    #.##.####.#
    #.##.####.#
    """, null, 16)]
    public void FindMirrorTest(string input, int? x, int? y)
    {
        var grid = new Grid(Read.String(input));
        var result = grid.FindMirror();
        Assert.Equal((x, y), result);
    }
}