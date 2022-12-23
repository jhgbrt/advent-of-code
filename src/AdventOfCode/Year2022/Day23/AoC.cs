using AdventOfCode.Common;

using Microsoft.CodeAnalysis;

namespace AdventOfCode.Year2022.Day23;
using static Direction;
#pragma warning disable CS8524 


public class AoC202223
{
    static bool usesample = false;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();
    static string[] theinput = usesample ? sample : input;
    public object Part1() => Grid.Parse(theinput).DoMoves(10).Count();
    public object Part2() => Grid.Parse(theinput).DoMovesWhileMoving();
}


class Grid
{
    ImmutableHashSet<Coordinate> _items;
    (Direction a, Direction b, Direction c, Direction d) _directions;


    public Grid(ImmutableHashSet<Coordinate> items, (Direction a, Direction b, Direction c, Direction d) directions)
    {
        _items = items;
        _directions = directions;
        var agg = _items.Aggregate(new Coordinate(int.MaxValue, int.MaxValue), (o, p) => o with { x = Min(o.x, p.x), y = Min(o.y, p.y) });
        BottomLeft = new(Min(0, agg.x), Min(0, agg.y));
        TopRight = _items.Aggregate(new Coordinate(int.MinValue, int.MinValue), (o, p) => o with { x = Max(o.x, p.x), y = Max(o.y, p.y) });
    }

    private Coordinate BottomLeft { get; }

    private Coordinate TopRight { get; }

    private int Size => (TopRight.y - BottomLeft.y + 1) * (TopRight.x - BottomLeft.x + 1);

    internal int Count() => Size - _items.Count;

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

    public int DoMovesWhileMoving()
    {
        int n = 0;
        var grid = this;
        while (true)
        {
            var next =  grid.DoMoves();
            n++;
            if (grid._items.SequenceEqual(next._items))
                return n;
            grid = next;
        }
    }

    public Grid DoMoves() => new Grid((from c in _items
                                       let proposed = (
                                           from d in _directions.AsEnumerable()
                                           where CanMove(c, d)
                                           select c.Move(d) as Coordinate?
                                       ).FirstOrDefault() ?? c
                                       group c by proposed into g
                                       from item in g.Skip(1).Any() ? g : g.Key.AsEnumerable()
                                       select item).ToImmutableHashSet(), (_directions.b, _directions.c, _directions.d, _directions.a));

    bool CanMove(Coordinate c, Direction d) => c.AllNeighbours().Any(c => this[c] == '#') && d switch
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