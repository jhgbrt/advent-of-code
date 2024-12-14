using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2024.Day14;

public class AoC202414(string[] input, TextWriter writer, int width, int height)
{
    public AoC202414() : this(Read.InputLines(), Console.Out, 101, 103) {}

    ImmutableArray<Robot> robots = ReadInput(input).ToImmutableArray();

    static IEnumerable<Robot> ReadInput(string[] lines) =>
        from t in lines.Index()
        select Robot.Parse(t.Index, t.Item);

    public int Part1() => (
        from r in robots
        let m = r.Move(100, width, height)
        group m by m.p.GetQuadrant(width, height) into g
        where g.Key != Quadrant.None
        select g.Count()
    ).Aggregate(1, (i, c) => i * c);

    public int Part2()
    {
        Span<Robot> span = robots.ToArray();
        var set = new HashSet<Coordinate>();
        int n = 0;
        (int top, int left, int bottom, int right) bounds;
        do
        {
            n++;
            set.Clear();
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = span[i].Move(1, width, height);
                set.Add(span[i].p);
            }
            if (n % 10000 == 0) writer.WriteLine(n);
        } while (!IsChristmasTree(set, out bounds));

        DrawImage(bounds, set);

        return n;
    }

    bool IsChristmasTree(HashSet<Coordinate> set, out (int top, int left, int bottom, int right) bounds)
    {
        bounds = (-1, -1, -1, -1);
        // optimization (cf. subreddit)
        if (set.Count != robots.Length) return false;
        bounds = GetBounds(set, 30); // 30 is arbitrary, but turns out to be ok
        return bounds is (>0, > 0, > 0, > 0);
    }

    (int top, int left, int bottom, int right) GetBounds(HashSet<Coordinate> set, int size)
    {
        var (top, left, bottom, right) = (-1, -1, -1, -1);
        for (int y = 0; y < height && bottom < 0; y++)
        {
            // find sequence of adjacent robots in this row of at least 10
            for (int x = 0; x < width - size; x++)
            {
                if (Range(x, size).All(i => set.Contains(new Coordinate(i, y))))
                {
                    (top, bottom) = (top, bottom) switch
                    {
                        ( < 0, _) => (y, bottom),
                        (_, < 0) => (top, y),
                    };
                    break;
                }
            }
        }
        for (int x = 0; x < width && right < 0; x++)
        {
            // find sequence of adjacent robots in this column of at least 10
            for (int y = 0; y < height - size; y++)
            {
                if (Range(y, size).All(i => set.Contains(new Coordinate(x, i))))
                {
                    (left, right) = (left, right) switch
                    {
                        ( < 0, _) => (x, right),
                        (_, < 0) => (left, x),
                    };
                    break;
                }
            }
        }
        return (top, left, bottom, right);
    }



    void DrawImage((int top, int left, int bottom, int right) bounds, HashSet<Coordinate> set)
    {
        var (top, left, bottom, right) = bounds;
        Console.WriteLine((top, left, bottom, right));
        var r = new Random();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var c = new Coordinate(x, y);
                if (set.Contains(c))
                {
                    if (x < left || x > right || y < top || y > bottom)
                    {
                        Span<ConsoleColor> colors = [ConsoleColor.Gray, ConsoleColor.Yellow, ConsoleColor.DarkGray, ConsoleColor.White, ConsoleColor.DarkYellow];
                        Write('*', colors[r.Next(0, colors.Length - 1)]);

                    }
                    else if ((x, y) == (left, top) || ((x, y) == (left, bottom)) || ((x, y) == (right, top) || (x, y) == (right, bottom)))
                    {
                        Write('+', ConsoleColor.DarkGray);
                    }
                    else if (x == left || x == right)
                    {
                        Write('|', ConsoleColor.DarkGray);
                    }
                    else if (y == top || y == bottom)
                    {
                        Write('-', ConsoleColor.DarkGray);
                    }
                    else
                    {
                        Write('#', ConsoleColor.Green);
                    }
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
        Console.ResetColor();
    }

    static void Write(char c, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write(c);
        Console.ResetColor();
    }

}

class FiniteGrid : IReadOnlyDictionary<Coordinate, char>
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

    public IEnumerable<Coordinate> Keys
    {
        get
        {
            for (int y = origin.y; y < Height; y++)
                for (int x = origin.x; x < endmarker.x; x++)
                    yield return new Coordinate(x, y);
        }
    }

    public IEnumerable<char> Values => Keys.Select(k => this[k]);

    public int Count => Width * Height;

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

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];


    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < endmarker.x && p.y < endmarker.y;


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

    public bool ContainsKey(Coordinate key) => IsValid(key);

    public bool TryGetValue(Coordinate key, [MaybeNullWhen(false)] out char value)
    {
        if (IsValid(key))
        {
            value = this[key];
            return true;
        }
        value = empty;
        return true;
    }

    public IEnumerator<KeyValuePair<Coordinate, char>> GetEnumerator() => Keys.Select(k => new KeyValuePair<Coordinate, char>(k, this[k])).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

record struct Delta(int x, int y);
record struct Coordinate(int x, int y)
{
    public static Coordinate operator +(Coordinate c, Delta d) => new(c.x + d.x, c.y + d.y);
    internal Quadrant GetQuadrant(int width, int height) => (x - width / 2, y - height / 2) switch
    {
        ( > 0, < 0) => Quadrant.NE,
        ( > 0, > 0) => Quadrant.SE,
        ( < 0, > 0) => Quadrant.SW,
        ( < 0, < 0) => Quadrant.NW,
        (0, _) or (_, 0) => Quadrant.None
    };
}
record struct Velocity(int dx, int dy)
{
    public static Delta operator *(Velocity v, int t) => new(v.dx * t, v.dy * t);
}
public enum Quadrant
{
    NW, NE, SE, SW, None
}
record struct Robot(int id, Coordinate p, Velocity v)
{
    public static Robot Parse(int id, string s)
    {
        var m = Regexes.Robot().Match(s);
        return new Robot(
            id,
            new (int.Parse(m.Groups["x"].Value), int.Parse(m.Groups["y"].Value)),
            new (int.Parse(m.Groups["dx"].Value), int.Parse(m.Groups["dy"].Value))
        );
    }
    public Robot Move(int t, int w, int h)
    {
        var pos = p + (v * t);
        pos = pos with { x = (pos.x + t*w) % w, y = (pos.y+t*h) % h };
        return this with { p = pos };
    }
}

static partial class Regexes
{
    [GeneratedRegex(@"p=(?<x>[\d]+),(?<y>[\d]+) v=(?<dx>[-+\d]+),(?<dy>[-+\d]+)")]
    public static partial Regex Robot();
}


public class AoC202414Tests
{
    private readonly AoC202414 sut;
    public AoC202414Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202414(input, new TestWriter(output), 11, 7);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(12, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }

    [Theory]
    [InlineData(0, 0, 11, 7, Quadrant.NW)]
    [InlineData(3, 2, 11, 7, Quadrant.NW)]
    [InlineData(7, 0, 11, 7, Quadrant.NE)]
    [InlineData(10, 4, 11, 7, Quadrant.SE)]
    [InlineData(0, 7, 11, 7, Quadrant.SW)]
    [InlineData(10, 0, 11, 7, Quadrant.NE)]
    [InlineData(0, 4, 11, 7, Quadrant.SW)]
    [InlineData(10, 7, 11, 7, Quadrant.SE)]

    public void GetQuadrant(int x, int y, int width, int height, Quadrant expected)
    {
        var c = new Coordinate(x, y);
        var q = c.GetQuadrant(width, height);
        Assert.Equal(expected, q);
    }

    [Theory]
    [InlineData(1, 4, 1)]
    [InlineData(2, 6, 5)]
    [InlineData(3, 8, 2)]
    [InlineData(4, 10, 6)]
    [InlineData(5, 1, 3)]
    public void RobotMove(int t, int x, int y)
    {
        var robot = new Robot(0, new Coordinate(2, 4), new Velocity(2, -3));

        var moved = robot.Move(t, 11, 7);
        Assert.Equal(new Coordinate(x, y), moved.p);
    }

}