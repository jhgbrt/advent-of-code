﻿using Spectre.Console;

namespace AdventOfCode;

enum Direction{ N, NE, E, SE, S, SW, W, NW }


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

    internal FiniteGrid(ImmutableDictionary<Coordinate,char> items, char empty, Coordinate endmarker)
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
        get
        {
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
            from y in new[]{p.y - 1, p.y, p.y + 1}
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


class InfiniteGrid
{
    readonly Dictionary<Coordinate, char> items = new();
    readonly char empty = '.';
    public char this[Coordinate p]
    {
        get
        {
            return items.TryGetValue(p, out var c) ? c : empty;
        }
        set
        {
            if (value == empty)
            {
                items.Remove(p);
            }
            else
            {
                items[p] = value;
            }
        }
    }

    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    private Coordinate topleft => new(items.Keys.Min(x => x.x), items.Keys.Min(x => x.y));
    private Coordinate bottomright => new(items.Keys.Max(x => x.x), items.Keys.Max(x => x.y));

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = topleft.y; y <= bottomright.y; y++)
        {
            for (int x = topleft.x; x <= bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public FiniteGrid ToFinite()
    {
        var deltaX = topleft.x < 0 ? -topleft.x : 0;
        var deltaY = topleft.y < 0 ? -topleft.y : 0;

        var items = this.items.ToImmutableDictionary(
                   x => new Coordinate(
                    x.Key.x + deltaX, x.Key.y + deltaY),
                    x => x.Value
        );

        return new FiniteGrid(
        items,
        empty,
        new(bottomright.x + deltaX + 1, bottomright.y + deltaY + 1)
    );
    }

    public int Count() => items.Count;
}

class InfiniteGrid3D
{
    readonly Dictionary<Coordinate3D, char> items = new();
    readonly char empty = '.';
    public char this[Coordinate3D p]
    {
        get {
            return items.TryGetValue(p, out var c) ? c : empty;
        }
        set {
            if (value == empty)
            {
                items.Remove(p);
            }
            else
            {
                items[p] = value;
            }
        }
    }

    public char this[(int x, int y, int z) p] => this[new Coordinate3D(p.x, p.y, p.z)];
    public char this[int x, int y, int z] => this[new Coordinate3D(x, y, z)];

    private Coordinate3D origin => new(
        items.Keys.Min(x => x.x), 
        items.Keys.Min(x => x.y),
        items.Keys.Min(x => x.z)
        );
    private Coordinate3D target => new(
        items.Keys.Max(x => x.x), 
        items.Keys.Max(x => x.y),
        items.Keys.Max(x => x.z)
        );

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int z = origin.z; z <= target.z; z++)
        {
            sb.AppendLine($"z={z}");
            for (int y = origin.y; y <= target.y; y++)
            {
                for (int x = origin.x; x <= target.x; x++)
                    sb.Append(this[x, y, z]);
                sb.AppendLine();
            }
            sb.AppendLine();
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
