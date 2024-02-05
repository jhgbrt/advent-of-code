using Microsoft.CodeAnalysis;

using System.Collections;

namespace AdventOfCode.Year2019.Day18;
public class AoC201918(string[] input)
{
    TextWriter? writer;
    public AoC201918() : this(Read.InputLines(), Console.Out) { }
    public AoC201918(string[] input, TextWriter? writer) : this(input) { this.writer = writer; }

    readonly Grid grid = new Grid(input);

    public int Part1() => new PathFinder(grid, new('@')).ShortestPath();

    public int Part2()
    {
        var bots = new Bots('!', '@', '$', '%');
        var pathFinder = new PathFinder(grid.With(
        b =>
        {
            var origin = grid.Find('@');
            b[origin] = '#';
            b[origin with { X = origin.X + 1 }] = '#';
            b[origin with { X = origin.X - 1 }] = '#';
            b[origin with { Y = origin.Y + 1 }] = '#';
            b[origin with { Y = origin.Y - 1 }] = '#';
            foreach (var (c, p) in bots.Zip([
                origin + (-1, -1),
                    origin + (1, -1),
                    origin + (1, 1),
                    origin + (-1, 1)
                ]))
                b[p] = c;
        }), bots);
        return pathFinder.ShortestPath();
    }
    
}


public class AoC201918Tests
{
    private readonly ITestOutputHelper output;

    public AoC201918Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC201918(input, new TestWriter(output));
    }

    [Theory]
    [InlineData(1, 8)]
    [InlineData(2, 86)]
    [InlineData(3, 132)]
    [InlineData(4, 136)]
    [InlineData(5, 81)]
    public void TestPart1(int n, int expected)
    {
        var input = Read.SampleLines(n);
        var sut = new AoC201918(input, new TestWriter(output));
        Assert.Equal(expected, sut.Part1());
    }

    [Theory]
    [InlineData(6, 8)]
    [InlineData(7, 24)]
    //[InlineData(8, 32)]
    [InlineData(9, 72)]
    public void TestPart2(int n, int expected)
    {
        var input = Read.SampleLines(n);
        var sut = new AoC201918(input, new TestWriter(output));
        Assert.Equal(expected, sut.Part2());
    }
}


class PathFinder
{
    readonly Grid grid;
    readonly Dictionary<char, Coordinate> keyCoordinates;
    readonly ImmutableDictionary<(char a, char b), int> keyDistances;
    readonly ImmutableDictionary<(char a, char b), DoorCollection> requiredDoors;
    readonly Bots bots;
    public PathFinder(Grid grid, Bots bots)
    {
        this.grid = grid;
        this.bots = bots;
        keyCoordinates = (from item in grid.ReverseLookup
                     where char.IsLower(item.Key) || bots.Contains(item.Key)
                     select (c: item.Key, p: item.Single())).ToDictionary(x => x.c, x => x.p);

        var requiredDoors = new Dictionary<(char, char), DoorCollection>();
        var keyDistances = new Dictionary<(char, char), int>();
        ProcessKeys(requiredDoors, keyDistances);

        this.keyDistances = keyDistances.ToImmutableDictionary();
        this.requiredDoors = requiredDoors.ToImmutableDictionary();
        foreach (var b in bots) keyCoordinates.Remove(b);
    }

    public int ShortestPath() => ShortestPath(new State(bots)).Distance;
    State ShortestPath(State start)
    {
        HashSet<State> seen = [];
        PriorityQueue<State, int> queue = new();
        queue.Enqueue(start, start.Distance);
        long stateCount = 0;

        while (queue.TryDequeue(out var state, out _))
        {
            stateCount++;
            if (state.HasAllKeys(keyCoordinates.Keys))
            {
                return state;
            }

            if (seen.Contains(state))
            {
                continue;
            }
            else
            {
                seen.Add(state);
            }

            foreach (var (index, c) in GetReachableKeys(state, requiredDoors))
            {
                var distance = keyDistances[(state.Bots[index], c)];
                var newState = state.Increase(distance, index, c);
                queue.Enqueue(newState, newState.Distance);

            }

        }

        return default;
    }

    // For each key & the starting bot(s), calculate distance and required doors to reach every other key
    void ProcessKeys(IDictionary<(char, char), DoorCollection> requiredDoors, IDictionary<(char, char), int> keyDistances)
    {
        foreach (var (a, apos) in keyCoordinates)
        {
            var floodmap = GetFloodMap(keyCoordinates[a]);

            foreach (var (b, bpos) in keyCoordinates)
            {
                if (b == a)
                {
                    continue;
                }
                if (bots.Length > 1 && grid.GetQuadrant(apos) != grid.GetQuadrant(bpos))
                {
                    continue;
                }

                keyDistances[(a, b)] = floodmap[keyCoordinates[b]];

                Coordinate p = bpos;
                while (p != keyCoordinates[a])
                {
                    p = (from n in grid.Neighbours(p)
                         where grid[n] != '#'
                         orderby floodmap[n]
                         select n).First();

                    var c = grid[p];

                    if (char.IsAsciiLetterUpper(c))
                    {
                        if (!requiredDoors.TryGetValue((a, b), out var v)) v = new();
                        requiredDoors[(a, b)] = v.Add(c);
                    }
                }

            }
        }
    }

    Dictionary<Coordinate, int> GetFloodMap(Coordinate startPoint)
    {
        var floodmap = new Dictionary<Coordinate, int>(grid.Width * grid.Height);
        var seen = new HashSet<Coordinate>(grid.Width * grid.Height);

        Queue<Coordinate> queue = new Queue<Coordinate>();

        queue.Enqueue(startPoint);
        seen.Add(startPoint);
        while (queue.Count > 0)
        {
            var p = queue.Dequeue();

            foreach (var point in grid.Neighbours(p))
            {
                if (grid[point] != '#' && !seen.Contains(point))
                {
                    if (!floodmap.TryGetValue(p, out int v))
                        v = floodmap[p] = 0;
                    floodmap[point] = v + 1;
                    queue.Enqueue(point);
                    seen.Add(point);
                }
            }
        }
        return floodmap;
    }

    IEnumerable<(int, char)> GetReachableKeys(State state, IReadOnlyDictionary<(char, char), DoorCollection> requiredDoors)
    {
        for (var bot = 0; bot < bots.Length; bot++)
        {
            foreach (var (key, v) in keyCoordinates)
            {
                if (
                    state.HasKey(key)
                    && (bots.Length == 1 || grid.GetQuadrant(v) == bot)
                    && state.HasAllKeysForDoors(requiredDoors.GetValueOrDefault((state.Bots[bot], key)))
                    )
                {
                    yield return (bot, key);
                }
            }
        }
    }
}

readonly record struct DoorCollection(int Value)
{
    internal DoorCollection Add(char door) => door switch { >= 'A' and <= 'Z' => this with { Value = Value | 1 << (door - 'A') }, _ => throw new InvalidOperationException() };
    internal bool AllKeysPresent(int keymask) => (keymask & Value) == Value;
}



struct State
{
    readonly int keymask;

    public State(Bots bots, int keymask = 0)
    {
        Bots = bots;
        Distance = 0;
        this.keymask = keymask;
    }
    internal bool HasKey(char key) => (keymask & (1 << (key - 'a'))) == 0;
    internal bool HasAllKeys(IReadOnlyCollection<char> keys) => keymask == (1 << keys.Count) - 1;
    internal bool HasAllKeysForDoors(DoorCollection doors) => doors.AllKeysPresent(keymask);
    public int Distance { get; private set; }
    public Bots Bots { get; }
    public override bool Equals(object? obj) => (obj is State state) && (Bots, keymask) == (state.Bots, state.keymask);

    public override int GetHashCode() => (Bots, keymask).GetHashCode();

    internal State Increase(int distance, int index, char bot) => new(Bots.Set(index, bot), keymask | (1 << (bot - 'a')))
    {
        Distance = Distance + distance
    };

}

readonly record struct Bots(char bot, (char a, char b, char c, char d) bots, int index) : IEnumerable<char>
{
    public Bots(char bot) : this(bot, default, 0) { }
    public Bots(char a, char b, char c, char d) : this(default, (a, b, c, d), 1) { }
    private Bots((char a, char b, char c, char d) bots) : this(default, bots, 1) { }
    public int Length => index switch
    {
        0 => 1,
        1 => 4,
        _ => throw new InvalidOperationException()
    };
    public Bots Set(int i, char value) => (index, i) switch
    {
        (0, 0) => new(value),
        (1, 0) => new(bots with { a = value }),
        (1, 1) => new(bots with { b = value }),
        (1, 2) => new(bots with { c = value }),
        (1, 3) => new(bots with { d = value }),
        _ => throw new IndexOutOfRangeException()
    };

    public IEnumerator<char> GetEnumerator() => index switch
    {
        0 => bot.AsEnumerable().GetEnumerator(),
        1 => bots.AsEnumerable().GetEnumerator(),
        _ => throw new InvalidOperationException()
    };

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public char this[int i] => (index, i) switch
    {
        (0, 0) => bot,
        (1, 0) => bots.a,
        (1, 1) => bots.b,
        (1, 2) => bots.c,
        (1, 3) => bots.d,
        _ => throw new IndexOutOfRangeException()
    };

}


readonly record struct Coordinate(int X, int Y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({X},{Y})";

    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.X + p.dx, left.Y + p.dy);

}

class Grid
{
    readonly ImmutableDictionary<Coordinate, char> items;
    readonly ILookup<char, Coordinate> reverse;
    public ILookup<char, Coordinate> ReverseLookup => reverse;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.Y;
    public int Width => endmarker.X;
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
        this.reverse = items.ToLookup(x => x.Value, x => x.Key);
        this.empty = empty;
        this.endmarker = endmarker;
    }
    public Coordinate Find(char c) => items.Single(kvp => kvp.Value == c).Key;
    public int Count(char c) => items.Count(kvp => kvp.Value == c);

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];


    public IEnumerable<Coordinate> Neighbours(Coordinate p)
    => from d in new (int x, int y)[]
       {
                (-1, 0),
                (0, 1),
                (1, 0),
                (0, -1)
       }
       where Contains(p + d)
       select p + d;

    public Grid With(Action<ImmutableDictionary<Coordinate, char>.Builder> action)
    {
        var builder = items.ToBuilder();
        action(builder);
        return new Grid(builder.ToImmutable(), empty, endmarker);
    }


    public bool Contains(Coordinate p) => p.X >= 0 && p.Y >= 0 && p.X < endmarker.X && p.Y < endmarker.Y;

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.Y; y < endmarker.Y; y++)
        {
            for (int x = origin.X; x < endmarker.X; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    /// <summary>
    ///  0|1
    ///  ---
    ///  3|2
    /// </summary>
    public int GetQuadrant(Coordinate c) => (isLeft: c.X < (Width / 2), isTop: c.Y < (Height / 2)) switch
    {
        (true, true) => 0,
        (false, true) => 1,
        (false, false) => 2,
        (true, false) => 3,
    };
}
