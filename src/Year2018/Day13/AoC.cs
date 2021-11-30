namespace AdventOfCode.Year2018.Day13;

public class AoC201813 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201813));
    public override object Part1()
    {
        (var grid, var start) = BuildGrid();

        (var maxX, var maxY) = (grid.Keys.Max(k => k.X), grid.Keys.Max(k => k.Y));

        var carts = start.ToList();

        while (true)
        {
            List<Cart> nextcarts = new();

            foreach (var cart in (from c in carts orderby c.Location.Y, c.Location.X select c))
            {
                var next = cart.Walk(grid);

                if (carts.Any(c => c.Location == next.Location))
                {
                    return $"{next.Location.X},{next.Location.Y}";
                }

                nextcarts.Add(next);
            }
            carts = nextcarts;
        }
    }

    public override object Part2()
    {
        (var grid, var start) = BuildGrid();

        var carts = start.ToList();
        while (carts.Count > 1)
        {
            List<Cart> nextcarts = new();
            var orderedCarts = carts.OrderBy(c => c.Location.Y).ThenBy(c => c.Location.X).ToList();
            while (orderedCarts.Any())
            {
                var cart = orderedCarts[0];
                orderedCarts.RemoveAt(0);

                var next = cart.Walk(grid);

                var collisions = orderedCarts.Concat(nextcarts).Where(c => c.Location == next.Location).ToArray();

                if (collisions.Any())
                {
                    foreach (var c in collisions)
                    {
                        orderedCarts.Remove(c);
                        nextcarts.Remove(c);
                    }
                }
                else
                {
                    nextcarts.Add(next);
                }
            }

            carts = nextcarts;
        }

        (var x, var y) = carts.Single().Location;
        return $"{x},{y}";
    }

    private static void Write(ImmutableDictionary<Coordinate, char> grid, ImmutableArray<Cart> carts)
    {
        Console.Clear();
        var cartsByLoc = carts.ToDictionary(x => x.Location);
        for (int y = 0; y < input.Length; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                var c = new Coordinate(x, y);
                if (cartsByLoc.ContainsKey(c))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(cartsByLoc[c].Symbol);
                    Console.ResetColor();
                }
                else if (grid.ContainsKey(c))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(grid[c]);
                    Console.ResetColor();
                }
                else Console.Write(' ');
            }
            Console.WriteLine();
        }
    }

    private static (ImmutableDictionary<Coordinate, char> grid, ImmutableArray<Cart> carts) BuildGrid()
    {
        var grid = (
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[y].Length)
            where input[y][x] != ' '
            select (x, y, c: input[y][x])
            ).ToImmutableDictionary(i => new Coordinate(i.x, i.y), i => i.c);

        var cartSymbols = new[] { '<', '>', 'v', '^' };

        int id = 0;
        var carts = (
            from kv in grid
            where cartSymbols.Contains(kv.Value)
            select new Cart(kv.Key, ++id, kv.Value, AtCrossing.TurnLeft)
        ).OrderBy(c => c.Location.Y).ThenBy(c => c.Location.X).ToImmutableArray();

        var builder = grid.ToBuilder();
        foreach (var cart in carts)
        {
            builder[cart.Location] = cart.Symbol switch
            {
                '^' or 'v' => '|',
                '<' or '>' => '-',
                _ => throw new NotImplementedException()
            };
        }
        grid = builder.ToImmutable();

        return (grid, carts);
    }


}
record Coordinate(int X, int Y);

record Cart(Coordinate Location, int Id, char Symbol, AtCrossing AtNextCrossing)
{
    public Cart Walk(ImmutableDictionary<Coordinate, char> grid)
    {
        return grid[Next()] switch
        {
            '|' or '-' => Straight(),
            '/' => Symbol switch { '^' or 'v' => TurnRight(), '<' or '>' => TurnLeft(), _ => throw new Exception() },
            '\\' => Symbol switch { '^' or 'v' => TurnLeft(), '<' or '>' => TurnRight(), _ => throw new Exception() },
            '+' => AtNextCrossing switch
            {
                AtCrossing.TurnLeft => TurnLeft() with { AtNextCrossing = AtCrossing.GoStraight },
                AtCrossing.GoStraight => Straight() with { AtNextCrossing = AtCrossing.TurnRight },
                AtCrossing.TurnRight => TurnRight() with { AtNextCrossing = AtCrossing.TurnLeft },
                _ => throw new NotImplementedException()
            },
            _ => throw new NotImplementedException()
        };
    }

    Coordinate Next() => Symbol switch
    {
        '>' => new(Location.X + 1, Location.Y),
        '<' => new(Location.X - 1, Location.Y),
        'v' => new(Location.X, Location.Y + 1),
        '^' => new(Location.X, Location.Y - 1),
        _ => throw new NotImplementedException()
    };

    Cart Straight() => this with
    {
        Location = Next()
    };

    Cart TurnLeft() => this with
    {
        Location = Next(),
        Symbol = Symbol switch
        {
            '>' => '^',
            '^' => '<',
            '<' => 'v',
            'v' => '>',
            _ => throw new NotImplementedException()
        }
    };

    Cart TurnRight() => this with
    {
        Location = Next(),
        Symbol = Symbol switch
        {
            '>' => 'v',
            'v' => '<',
            '<' => '^',
            '^' => '>',
            _ => throw new NotImplementedException()
        }
    };
}
enum AtCrossing { TurnLeft, GoStraight, TurnRight };
