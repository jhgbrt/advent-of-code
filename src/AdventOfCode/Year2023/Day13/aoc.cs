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
        var q = from grid in items
                let mirror = grid.FindMirror()
                select mirror.x.HasValue ? mirror.x.Value
                : mirror.y.HasValue 
                ? mirror.y.Value * 100
                : 0;

        return q.Sum();
    }
    public object Part2() => "";
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
        foreach (var col in Range(1, Width - 1))
        {
            for (int x = col; x >= 0; x--)
            {
                if (Col(col - x) != Col(col + x - 1)) break;
            }

            if (Range(0, Height).All(y => IsSymmetricRow(y, col)))
            {
                return (col + 1, null);
            }
        }
        foreach (var row in Range(0, Height))
        {
            if (Range(0, Width).All(x => IsSymmetricColumn(x, row)))
            {
                return (null, row + 1);
            }
        }

        return (null, null);
    }

    bool IsSymmetricRow(int y, int center)
    {
        for (int offset = 0; offset <= center; offset++)
        {
            var x1 = center - offset;
            var x2 = center + offset + 1;
            if (Contains(new(x1,y)) && Contains(new(x2, y)) && this[x1, y] != this[x2, y])
            {
                return false;
            }
        }
        return true;
    }

    bool IsSymmetricColumn(int x, int center)
    {
        for (int offset = 0; offset <= center; offset++)
        {
            var y1 = center - offset;
            var y2 = center + offset + 1;
            if (Contains(new(x, y1)) && Contains(new(x, y2)) && this[x, y1] != this[x, y2])
            {
                return false;
            }
        }
        return true;
    }

    string Row(int y) => new string(Range(0, Width).Select(i => this[i, y]).ToArray());
    string Col(int x) => new string(Range(0, Height).Select(i => this[x, i]).ToArray());

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

    public bool Contains(Coordinate c) => c.x >= 0 && c.x < bottomright.x && c.y >=0 && c.y < bottomright.y;

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

    [Fact]
    public void TestPart2()
    {
        var input = Read.Sample(1).Lines().ToArray();
        AoC202313 sut = new AoC202313(output, input);
        Assert.Equal(string.Empty, sut.Part2());
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
    public void FindMirrorTest(string input, int? x, int? y)
    {
        var grid = new Grid(Read.String(input));
        var result = grid.FindMirror();
        Assert.Equal((x, y), result);
    }
}