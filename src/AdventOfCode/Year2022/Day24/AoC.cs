using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day24;
public class AoC202224
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();
    static string[] theinput = usesample ? sample : input;
    public object Part1()
    {
        var start = new Coordinate(1, 0);
        var target = new Coordinate(theinput[0].Length - 2, theinput.Length - 1);
        var grid = new Grid(theinput);
        return FindPath(grid, new(), new(1,0), target);
    }


    private (int, bool) FindPath(Grid grid, HashSet<(Coordinate, string)> seen, Coordinate current, Coordinate target)
    {
        var key = (current, grid.ToString());

        if (seen.Contains(key))
            return (int.MaxValue, false);

        seen.Add(key);

        int n = 1;
        grid.Draw(current);
        grid = grid.Move();

        while (!grid.OpenPositions(current).Any())
        {
            n++;
            grid.Draw(current);
            grid = grid.Move();
        }
        var reached = false;
        int best = int.MaxValue;
        foreach (var c in grid.OpenPositions(current))
        {
            if (c == target)
            {
                n += 1;
                reached = true;
            }
            else
            {
                (int i, reached) = FindPath(grid, seen, c, target);
                if (reached) 
                    n += i;
            }
            if (reached && best > n) best = n;
        }
        return (best, reached);
    }

    public object Part2() => "";
}

/// <summary>
/// A finite, immutable grid. Y increments downward, X increments rightward
/// </summary>
class Grid
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, List<char>> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    int Bottom => bottomright.y;
    int Right => bottomright.x;
    int Left => origin.x;
    int Top => origin.y;

    public Grid(string[] input, char empty = '.')
    {
        bottomright = new(input[0].Length - 1, input.Length - 1);
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 select (x, y, c: input[y][x]))
                 .ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c.AsEnumerable().ToList());
        this.empty = empty;
    }
    public Grid Move()
    {
        var blizzard = (from item in items
                        let c = item.Key
                        from v in item.Value
                        where v is '>' or '<' or 'v' or '^'
                        let n = v switch
                        {
                            '>' => c with { x = c.x == Right - 1 ? Left + 1 : c.x + 1 },
                            '<' => c with { x = c.x == Left + 1 ? Right - 1 : c.x - 1 },
                            '^' => c with { y = c.y == Top + 1 ? Bottom - 1 : c.y - 1 },
                            'v' => c with { y = c.y == Bottom - 1 ? Top + 1 : c.y + 1 },
                            _ => c
                        }
                        group v by n).ToDictionary(g => g.Key, g => g.ToList());

        var newitems = (
            from y in Range(0, Bottom + 1)
            from x in Range(0, Right + 1)
            let c = new Coordinate(x, y)
            let v = blizzard.ContainsKey(c)
            ? blizzard[c]
            : items[c] is ['#']
            ? '#'.AsEnumerable()
            : empty.AsEnumerable()
            select (c, v))
            .ToImmutableDictionary(t => t.c, t => t.v.ToList());

        return new Grid(newitems, empty, bottomright);

    }

    public IEnumerable<Coordinate> OpenPositions(Coordinate c)
        => from n in c.Neighbours()
           where items.TryGetValue(n, out var v) && v is ['.']
           select n;

    private Grid(ImmutableDictionary<Coordinate, List<char>> items, char empty, Coordinate bottomright)
    {
        this.items = items;
        this.empty = empty;
        this.bottomright = bottomright;
    }

    IReadOnlyList<char> this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty.AsEnumerable().ToList();
    IReadOnlyList<char> this[(int x, int y) p] => this[new Coordinate(p.x,p.y)];
    IReadOnlyList<char> this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, bottomright.y)
        from x in Range(origin.x, bottomright.x)
        select new Coordinate(x, y);

    public IEnumerable<Coordinate> InteriorPoints() =>
        from y in Range(origin.y + 1, bottomright.y - 2)
        from x in Range(origin.x + 1, bottomright.x - 2)
        select new Coordinate(x, y);
    public void Draw(Coordinate current)
    {
        //Console.Clear();
        for (int y = Top; y <= Bottom; y++)
        {
            for (int x = Left; x <= Right; x++)
            {
                if (current == (x,y))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write('E');
                    Console.ResetColor();
                }
                else
                {
                    var l = this[x, y];
                    var c = l.Count() == 1 ? l[0] : l.Count().ToString()[0];
                    Console.Write(c);
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = Top; y <= Bottom; y++)
        {
            for (int x = Left; x <= Right; x++)
            {
                var l = this[x, y];
                var c = l.Count() == 1 ? l[0] : l.Count().ToString()[0];
                sb.Append(c); 
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
    public static implicit operator Coordinate((int x, int y) t) => new(t.x, t.y);
    public static implicit operator (int x, int y) (Coordinate c) => new(c.x, c.y);
    public static Coordinate operator +(Coordinate c, Coordinate d) => new(c.x + d.x, c.y + d.y);
    public static Coordinate operator -(Coordinate c, Coordinate d) => new(c.x - d.x, c.y - d.y);
    public static Coordinate operator +(Coordinate c, (int x, int y) d) => new(c.x + d.x, c.y + d.y);
    public static Coordinate operator -(Coordinate c, (int x, int y) d) => new(c.x - d.x, c.y - d.y);
    public IEnumerable<Coordinate> Neighbours()
    {
        yield return this with { x = x + 1 };
        yield return this with { y = y + 1 };
        yield return this with { x = x - 1 };
        yield return this with { y = y - 1 };
    }
}
