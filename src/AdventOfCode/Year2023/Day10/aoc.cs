using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Year2023.Day10;

public class AoC202310
{
    static string[] input = Read.InputLines();
    Coordinate start;
    FiniteGrid grid;
    HashSet<Coordinate> loop; 

    public AoC202310()
    {
        (start, grid, loop) = CreateGrid(input);
    }

    internal AoC202310(string[] input)
    {
        (start, grid, loop) = CreateGrid(input);
    }

    public object Part1() => loop.Count % 2 == 0 ? loop.Count / 2 : loop.Count / 2 - 1;

    static (Coordinate start, FiniteGrid grid, HashSet<Coordinate> loop) CreateGrid(string[] input)
    {
        FiniteGrid grid = new FiniteGrid(input);
        Coordinate start = grid.Find('S');
        var connection = DetermineConnection(grid, start);
        grid = grid.With(b => b[start] = connection);

        var loop = FindPath(grid, start).ToHashSet();
        grid = grid.With(
            b =>
            {
                b[start] = connection;
                var notinloop = b.Keys.Where(c => !loop.Contains(c)).ToList();
                foreach (var item in notinloop)
                {
                    b[item] = '.';
                }
            }
        );
        return (start, grid, loop);
    }

    static private IEnumerable<Coordinate> FindPath(FiniteGrid grid, Coordinate start)
    {        
        var current = start;
        var next = ConnectedNeighbours(grid, current).First();
        Coordinate? previous = null;
        do
        {
            yield return current;
            next = ConnectedNeighbours(grid, current).First(c => c != previous);
            previous = current;
            current = next;
        } while (current != start);
    }


    static char DetermineConnection(FiniteGrid grid, Coordinate start)
    {
        return (from x in grid.Neighbours(start)
                    where (x.d, grid[x.c]) switch
                    {
                        (Direction.N, '|' or 'F' or '7') => true,
                        (Direction.E, '-' or 'J' or '7') => true,
                        (Direction.S, '|' or 'J' or 'L') => true,
                        (Direction.W, '-' or 'F' or 'L') => true,
                        _ => false
                    }
                    orderby x.d
                    select x.d
       ).ToTuple2() switch
        {
            (Direction.N, Direction.E) => 'L',
            (Direction.N, Direction.S) => '|',
            (Direction.N, Direction.W) => 'J',
            (Direction.E, Direction.S) => 'F',
            (Direction.E, Direction.W) => '-',
            (Direction.S, Direction.W) => '7',
        };
    }
    public object Part2() => (from p in grid.Points()
                              where grid[p] == '.'
                              let count = CountLinesCrossed(grid, p, loop)
                              where count % 2 == 1
                              select p).Count();

    int CountLinesCrossed(FiniteGrid grid, Coordinate p, HashSet<Coordinate> loop)
    {
        Func<State, Coordinate, Result> f = OutsideLoop;
        var state = new State(grid, p, loop, 0);
        var e = East(grid, p).GetEnumerator();
        while (e.MoveNext())
        {
            (f, state) = f(state, e.Current);
        }
        return state.Count;
    }

    static IEnumerable<Coordinate> East(FiniteGrid grid, Coordinate c)
    {
        var current = c;
        while (current.x < grid.Width)
        {
            yield return current;
            current = current.E;
        }
    }


    Result OutsideLoop(State state, Coordinate c) => state.Grid[c] switch
    {
        'F' => new(OutsideOnF, state),
        'L' => new(OutsideOnL, state),
        '|' => new(InsideLoop, state.AddCount()),
        '.' => new(OutsideLoop, state)
    };

    Result OutsideOnF(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(OutsideOnF, state),
        'J' => new(InsideLoop, state.AddCount()),
        '7' => new(OutsideLoop, state),
    };
    Result OutsideOnL(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(OutsideOnL, state),
        'J' => new(OutsideLoop, state),
        '7' => new(InsideLoop, state.AddCount())
    };
    Result InsideOnF(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(InsideOnF, state),
        'J' => new(OutsideLoop, state.AddCount()),
        '7' => new(InsideLoop, state),
    };
    Result InsideOnL(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(InsideOnL, state),
        'J' => new(InsideLoop, state),
        '7' => new(OutsideLoop, state.AddCount())
    };

    Result InsideLoop(State state, Coordinate c) => state.Grid[c] switch
    {
        'F' => new(InsideOnF, state),
        'L' => new(InsideOnL, state),
        '|' => new(OutsideLoop, state.AddCount()),
        '.' => new(InsideLoop, state)
    };

    record struct Result(Func<State,Coordinate,Result> Next, State State);
    
    record struct State(FiniteGrid Grid, Coordinate p, HashSet<Coordinate> Loop, int Count)
    {
        public State AddCount() => this with { Count = Count + 1 };
    }

    private static IEnumerable<Coordinate> ConnectedNeighbours(FiniteGrid grid, Coordinate c)
    => grid[c] switch
    {
        '|' => [c.N, c.S],
        '-' => [c.E, c.W],
        'L' => [c.N, c.E],
        'J' => [c.N, c.W],
        'F' => [c.S, c.E],
        '7' => [c.S, c.W],
        _ => Array.Empty<Coordinate>()
    };


}


public class Tests
{
    [Theory]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    [InlineData(4, 10)]
    public void Part2(int sample, int expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        var aoc = new AoC202310(input);
        Assert.Equal(expected, aoc.Part2());
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

    public IEnumerable<(Direction d, Coordinate c)> Neighbours(Coordinate p)
    {
        return
            from d in new (Direction direction, (int x, int y) delta)[]
            {
                (Direction.NW, (-1, -1)),
                (Direction.W, (-1, 0)),
                (Direction.SW, (-1, 1)),
                (Direction.S, (0, 1)),
                (Direction.SE, (1, 1)),
                (Direction.E, (1, 0)),
                (Direction.NE, (1, -1)),
                (Direction.N, (0, -1))
            }
            where IsValid(p + d.delta)
            select (d.direction, p + d.delta);
    }

    public Coordinate? GetNeighbour(Coordinate p, Direction d) => d switch
    {
        Direction.N => IfValid(new(p.x, p.y - 1)),
        Direction.NE => IfValid(new(p.x + 1, p.y - 1)),
        Direction.E => IfValid(new(p.x + 1, p.y)),
        Direction.SE => IfValid(new(p.x + 1, p.y + 1)),
        Direction.S => IfValid(new(p.x, p.y + 1)),
        Direction.SW => IfValid(new(p.x - 1, p.y + 1)),
        Direction.W => IfValid(new(p.x - 1, p.y)),
        Direction.NW => IfValid(new(p.x - 1, p.y - 1))
    };
    Coordinate? IfValid(Coordinate p) => IsValid(p) ? p : null;
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
