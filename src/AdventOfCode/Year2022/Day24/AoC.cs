using AdventOfCode.Common;

using System.Security.Cryptography;

namespace AdventOfCode.Year2022.Day24;
public class AoC202224
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();
    static string[] theinput = usesample ? sample : input;

    Coordinate start = new Coordinate(1, 0);
    Coordinate target = new Coordinate(theinput[0].Length - 2, theinput.Length - 1);
    Grid grid = new Grid(theinput);

    public object Part1() => FindPath(grid, start, target);


    public object Part2()
    {
        var t1 = FindPath(grid, start, target, 0);
        var t2 = FindPath(grid, target, start, t1);
        var t3 = FindPath(grid, start, target, t1 + t2);
        Console.WriteLine((t1, t2, t3));
        return t1 + t2 + t3;
    }

    int FindPath(Grid grid, Coordinate start, Coordinate destination, int t0 = 0)
    {
        var cache = new Dictionary<Coordinate, int>();

        cache[start] = 0;
        int t = 0;
        while (!cache.ContainsKey(destination))
        {
            var blocked = grid.BlockedAt(t0 + t + 1);

            var updated = (
                from p in grid.Points()
                where cache.TryGetValue(p, out var x) && x == t
                from n in p.AsEnumerable().Concat(p.Neighbours())
                where !blocked.Contains(n)
                select n
            ).ToList();

            foreach (var item in updated)
            {
                cache[item] = t + 1;
            }
            t++;
        }
        return cache[destination];
    }

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

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate bottomright;
    readonly char empty;
    public int Bottom => bottomright.y;
    public int Right => bottomright.x;
    int Left => origin.x;
    int Top => origin.y;

    public Grid(string[] input, char empty = '.')
    {
        bottomright = new(input[0].Length - 1, input.Length - 1);
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 let c = input[y][x]
                 where c != '.' || (x == 0 || y == 0 || x == Right || y == Bottom)
                 select (x, y, c))
                 .ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        
      
        this.empty = empty;
    }


    public ImmutableHashSet<Coordinate> BlockedAt(int t)
    {
        return (
            from kv in items
            let c = kv.Key
            let v = kv.Value
            where v is not '.'
            select v switch
            {
                '#' => c,
                '>' => c with { x = Mod(c.x - 1 + t, Right - 1) + 1 },
                '<' => c with { x = Mod(c.x - 1 - t, Right - 1) + 1 },
                'v' => c with { y = Mod(c.y - 1 + t, Bottom - 1) + 1 },
                '^' => c with { y = Mod(c.y - 1 - t, Bottom - 1) + 1 }
            }
            ).ToImmutableHashSet();
    }
    public bool Contains(Coordinate n) => items.ContainsKey(n);
    static T Mod<T>(T n, T m) where T : INumber<T> => (n % m + m) % m;

   

    public IEnumerable<Coordinate> OpenPositions(Coordinate c)
        => from n in c.Neighbours()
           where items.TryGetValue(n, out var v) && v is '.'
           select n;

    private Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate bottomright)
    {
        this.items = items;
        this.empty = empty;
        this.bottomright = bottomright;
    }

    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : '.';
    char this[(int x, int y) p] => this[new Coordinate(p.x,p.y)];
    char this[int x, int y] => this[new Coordinate(x, y)];

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
                    var c = this[x, y];
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
                var c = this[x, y];
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
