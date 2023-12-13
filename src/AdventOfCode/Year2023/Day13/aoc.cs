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
                select mirror.x.HasValue ? mirror.x.Value + 1
                : (mirror.y!.Value + 1)* 100;

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
        foreach (var center in Range(0, Width/2 + 1))
        {
            if (Range(0, Height).All(y => IsSymmetricRow(y, center)))
            {
                return (center, null);
            }
        }
        foreach (var center in Range(0, Height/2 + 1))
        {
            if (Range(0, Width).All(x => IsSymmetricColumn(x, center)))
            {
                return (null, center);
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
    private readonly AoC202313 sut;
    private TextWriter output;
    public AoC202313Tests(ITestOutputHelper helper)
    {
        output = new TestWriter(helper);
        var input = Read.SampleLines();
        sut = new AoC202313(output, input);
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(2, sut.Items.Count());
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(405, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}