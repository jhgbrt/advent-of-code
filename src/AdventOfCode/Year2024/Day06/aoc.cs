using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2024.Day06;
public class AoC202406
{
    public AoC202406(string[] input)
    {
        var g = new Grid(input);
        start = g.Find('^');
        grid = g.With(g => g[start] = '.');
    }
    public AoC202406() : this(Read.InputLines()) { }
    readonly Grid grid;
    readonly Coordinate start;

    public int Part1() => Traverse(grid, start).Select(x => x.c).Distinct().Count();

    public int Part2()
    {
        var reachable = Traverse(grid, start).Select(x => x.c).ToHashSet();
        var visited = new HashSet<(Coordinate, Direction)>(grid.Height*grid.Width); // reuse to reduce allocation pressure
        return (
            from obstruction in reachable
            where IsLoop(grid, start, visited, obstruction)
            select obstruction).Count();
    }

    static IEnumerable<(Coordinate c, Direction d)> Traverse(IReadOnlyDictionary<Coordinate, char> g, Coordinate start, Coordinate? obstruction = null)
    {
        var pos = start;
        var d = Direction.N;
        while (g.ContainsKey(pos))
        {
            yield return (pos, d);
            var next = pos + d == obstruction ? '#' : g[pos + d];
            (pos, d) = (next, d) switch
            {
                ('.', _) => (pos + d, d),
                ('#', Direction.N) => (pos, Direction.E),
                ('#', Direction.E) => (pos, Direction.S),
                ('#', Direction.S) => (pos, Direction.W),
                ('#', Direction.W) => (pos, Direction.N)
            };
        }
    }
    static bool IsLoop(IReadOnlyDictionary<Coordinate, char> grid, Coordinate start, HashSet<(Coordinate, Direction)> visited, Coordinate obstruction)
    {
        visited.Clear();
        foreach (var item in Traverse(grid, start, obstruction))
        {
            if (visited.Contains(item)) return true;
            visited.Add((item.c, item.d));
        }
        return false;
    }
}

public class AoC202406Tests
{
    private readonly AoC202406 sut;
    public AoC202406Tests(ITestOutputHelper output) => sut = new AoC202406(Read.SampleLines());

    [Fact]
    public void TestParsing() => Assert.NotNull(sut);

    [Fact]
    public void TestPart1() => Assert.Equal(41, sut.Part1());

    [Fact]
    public void TestPart2() => Assert.Equal(6, sut.Part2());

}


class Grid : IReadOnlyDictionary<Coordinate, char>
{
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

    public Grid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new Grid(builder.ToImmutable(), empty, endmarker);
    }
    public Coordinate Find(char c) => items.Where(i => i.Value == c).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
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

    public bool Contains(Coordinate c) => IsValid(c);

    public bool ContainsKey(Coordinate key) => IsValid(key);

    public bool TryGetValue(Coordinate key, [MaybeNullWhen(false)] out char value)
    {
        if (IsValid(key))
        {
            value = this[key];
            return true;
        }
        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<Coordinate, char>> GetEnumerator() => Keys.Select(k => new KeyValuePair<Coordinate, char>(k, this[k])).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

enum Direction { N, E, S, W }

readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate c, Direction d) => d switch
    {
        Direction.N => new (c.x, c.y - 1),
        Direction.E => new (c.x + 1, c.y),
        Direction.S => new (c.x, c.y + 1),
        Direction.W => new (c.x - 1, c.y)
    };
}
