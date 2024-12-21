using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2024.Day20;
public class AoC202420
{
    public AoC202420() : this(Read.InputLines(), Console.Out) { }
    readonly Grid grid;
    readonly IReadOnlyDictionary<Coordinate, int> path;

    public AoC202420(string[] input, TextWriter writer)
    {
        grid = new Grid(input);
        path = PathFromEnd().ToDictionary(x => x.c, x => x.d);
    }

    public int Part1() => FindCheats(2, path, 100).Sum(x => x.count);
    public int Part2() => FindCheats(20, path, 100).Sum(x => x.count);

    IEnumerable<(Coordinate c, int d)> PathFromEnd()
    {
        var distance = 0;
        var current = grid.Find('E');
        var target = grid.Find('S');
        yield return (current, distance);

        var previous = current;
        while (current != target)
        {
            (current, previous) = (grid.Neighbours(current).Single(next => next != previous && grid[next] != '#'), current);
            distance++;
            yield return (current, distance);
        }
    }

    internal IEnumerable<(int saved, int count)> FindCheats(int cheats, IReadOnlyDictionary<Coordinate, int> path, int min)
          => from item in path
             let @from = item.Key
             let time = item.Value
             from to in grid.RangeAround(@from, cheats)
             where path.ContainsKey(to)
             let saved = time - path[to] - to.Manhattan(@from)
             where saved >= min
             group (path[@from], path[to]) by saved into g
             select (g.Key, count: g.Count());

    internal int FindCheats(int cheats, int saved) => FindCheats(cheats, path, saved).Where(x => x.saved == saved).Sum(x => x.count);
}

public class AoC202420Tests
{
    private readonly AoC202420 sut;
    private readonly TestWriter writer;
    public AoC202420Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        writer = new TestWriter(output);
        sut = new AoC202420(input, writer);
    }

    [Fact]
    public void TestParsing()
    {
        
    }

    [Theory]
    [InlineData(14, 2)]
    [InlineData(14, 4)]
    [InlineData(2, 6)]
    [InlineData(4, 8)]
    [InlineData(2, 10)]
    [InlineData(3, 12)]
    public void TestPart1(int expected, int timeSaved)
    {
        Assert.Equal(expected, sut.FindCheats(2, timeSaved));
    }

    [Theory]
    [InlineData(32, 50)]
    [InlineData(31, 52)]
    [InlineData(29, 54)]
    [InlineData(39, 56)]
    [InlineData(25, 58)]
    [InlineData(23, 60)]
    [InlineData(20, 62)]
    [InlineData(19, 64)]
    [InlineData(12, 66)]
    [InlineData(14, 68)]
    [InlineData(12, 70)]
    [InlineData(22, 72)]
    [InlineData(4, 74)]
    [InlineData(3, 76)]

    public void TestPart2(int expected, int timeSaved)
    {
        Assert.Equal(expected, sut.FindCheats(20, timeSaved));
    }


}

class Grid : IReadOnlyDictionary<Coordinate, char>
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

    public Grid(string[] input, char empty = '.')
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {
    }
    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    internal Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }
    public Coordinate Find(char c) => items.Where(i => i.Value == c).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];


    static (int, int)[] deltas = [(0, 1), (1, 0), (0, -1), (-1, 0)];
    public IEnumerable<Coordinate> Neighbours(Coordinate p) => from d in deltas
                                                               where ContainsKey(p + d)
                                                               select p + d;

    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < Width && p.y < Height;

    public IEnumerable<Coordinate> RangeAround(Coordinate p, int range) => from y in Range(-range, 2 * range + 1)
                                                                           from x in Range(-range, 2 * range + 1)
                                                                           let c = p + (x, y)
                                                                           where IsValid(c) && c.Manhattan(p) <= range
                                                                           select c;
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++) sb.Append(this[x, y]);
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



readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
    public int Manhattan(Coordinate other) => Abs(x - other.x) + Abs(y - other.y);
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

