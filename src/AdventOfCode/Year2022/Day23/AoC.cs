using AdventOfCode.Common;
namespace AdventOfCode.Year2022.Day23;
using static Direction;
#pragma warning disable CS8524 


public class AoC202223
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();
    static string[] theinput = usesample ? sample : input;
    public object Part1()
    {
        Console.WriteLine(Grid.Parse(theinput));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(1));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(2));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(3));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(4));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(5));
        Console.WriteLine(Grid.Parse(theinput).DoMoves(10));
        return -1;
    }
    public object Part2() => "";
}


class Grid
{
    ImmutableHashSet<Coordinate> _items;
    (Direction a, Direction b, Direction c, Direction d) _directions;

    public Grid(ImmutableHashSet<Coordinate> items, (Direction a, Direction b, Direction c, Direction d) directions)
    {
        _items = items;
        _directions = directions;
    }

    private Coordinate BottomLeft
    {
        get
        {
            Coordinate agg = _items.Aggregate(new Coordinate(int.MaxValue, int.MaxValue), (o, p) => o with { x = Min(o.x, p.x), y = Min(o.y, p.y) });
            return new(Min(0, agg.x), Min(0, agg.y));
        }
    }

    private Coordinate TopRight
    {
        get
        {
            Coordinate agg = _items.Aggregate(new Coordinate(int.MinValue, int.MinValue), (o, p) => o with { x = Max(o.x, p.x), y = Max(o.y, p.y) });
            return new(agg.x + 1, agg.y + 1);
        }
    }

    internal int Count() => _items.Count();

    public static Grid Parse(string[] input)
    {
        return new Grid((
            from row in Range(0, input.Length)
            let y = input.Length - row - 1
            from x in Range(0, input[y].Length)
            where input[row][x] == '#'
            select new Coordinate(x, y)
        ).ToImmutableHashSet(), (N,S,W,E));
    }
    
    public char this[Coordinate c] => _items.Contains(c) ? '#' : '.';

    public Grid DoMoves(int n)
    {
        var grid = this;
        foreach (var _ in Repeat(0, n))
        {
            grid = grid.DoMoves();
        }
        return grid;
    }


    public Grid DoMoves() => new Grid((from c in _items
                                            let proposed = (
                                                from d in _directions.AsEnumerable()
                                                where CanMove(c, d)
                                                select c.Move(d) as Coordinate?
                                            ).FirstOrDefault()
                                            where proposed.HasValue
                                            group c by proposed.Value into g
                                            from item in g.Skip(1).Any() ? g : g.Key.AsEnumerable()
                                            select item).ToImmutableHashSet(), (_directions.b, _directions.c, _directions.d, _directions.a));

    public bool CanMove(Coordinate c, Direction d) => c.AllNeighbours().Any(c => this[c] == '#') && d switch
    {
        N => CanMoveNorth(c),
        E => CanMoveEast(c),
        S => CanMoveSouth(c),
        W => CanMoveWest(c)
    };

    public bool CanMoveNorth(Coordinate c) => (this[c.NorthWest()], this[c.North()], this[c.NorthEast()]) == ('.', '.', '.');
    public bool CanMoveSouth(Coordinate c) => (this[c.SouthWest()], this[c.South()], this[c.SouthEast()]) == ('.', '.', '.');
    public bool CanMoveWest(Coordinate c) => (this[c.NorthWest()], this[c.West()], this[c.SouthWest()]) == ('.', '.', '.');
    public bool CanMoveEast(Coordinate c) => (this[c.NorthEast()], this[c.East()], this[c.SouthEast()]) == ('.', '.', '.');

  
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = TopRight.y; y >= BottomLeft.y; y--)
        {
            for (int x = BottomLeft.x; x <= TopRight.x; x++)
            {
                sb.Append(_items.Contains(new(x, y)) ? '#' : '.');
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}


readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
    public Coordinate Move(Direction d) => d switch
    {
        N => North(),
        E => East(),
        S => South(),
        W => West()
    };

    public IEnumerable<Coordinate> AllNeighbours()
    {
        yield return North();
        yield return NorthEast();
        yield return East();
        yield return SouthEast();
        yield return South();
        yield return SouthWest();
        yield return West();
        yield return NorthWest();
    }

    public Coordinate North() => this with { y = y + 1 };
    public Coordinate South() => this with { y = y - 1 };
    public Coordinate East() => this with { x = x + 1 };
    public Coordinate West() => this with { x = x - 1 };
    public Coordinate SouthEast() => this with { x = x + 1, y = y - 1 };
    public Coordinate SouthWest() => this with { x = x - 1,  y = y - 1 };
    public Coordinate NorthEast() => this with { x = x + 1, y = y + 1 };
    public Coordinate NorthWest() => this with { x = x - 1, y = y + 1 };
}
enum Direction { N, E, S, W }