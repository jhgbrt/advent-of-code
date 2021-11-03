using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using static Direction;

var tiles = 
    from line in File.ReadLines("input.txt")
    select line.ToTile();

var flippedTiles = tiles.Aggregate(
    ImmutableHashSet<Tile>.Empty,
    (set, tile) => set.Contains(tile) ? set.Remove(tile) : set.Add(tile)
    );

var part1 = flippedTiles.Count;

for (int i = 0; i < 100; i++)
{
    var grid = (
        from x in flippedTiles
        from tile in new[] { x }.Concat(x.Neighbors())
        select (tile, flipped: flippedTiles.Contains(tile))
        ).Distinct();

    flippedTiles = grid.Aggregate(
        flippedTiles,
        (set, item) =>
            item switch
            {
                { flipped: true } when item.tile.Neighbors().Where(flippedTiles.Contains).Count() is 0 or > 2 => set.Remove(item.tile),
                { flipped: false } when item.tile.Neighbors().Where(flippedTiles.Contains).Count() is 2 => set.Add(item.tile),
                _ => set
            }
        );
}


var part2 = flippedTiles.Count;

Console.WriteLine((part1, part2));

record Tile(int x, int y, int z)
{
    public Tile East()      => this with { x = x + 1, y = y - 1 };
    public Tile West()      => this with { x = x - 1, y = y + 1 };
    public Tile SouthEast() => this with { y = y - 1, z = z + 1 };
    public Tile SouthWest() => this with { x = x - 1, z = z + 1 };
    public Tile NorthEast() => this with { x = x + 1, z = z - 1 };
    public Tile NorthWest() => this with { y = y + 1, z = z - 1 };
    public IEnumerable<Tile> Neighbors()
    {
        yield return new(x + 1, y - 1, z);
        yield return new(x + 1, y, z - 1);
        yield return new(x, y + 1, z - 1);
        yield return new(x - 1, y + 1, z);
        yield return new(x - 1, y, z + 1);
        yield return new(x, y - 1, z + 1);
    }
}

enum Direction { E, W, SE, SW, NE, NW };
static class AoC
{
    public static Tile ToTile(this string line)
    {
        var tile = new Tile(0, 0, 0);

        foreach (var d in GetDirections(line))
        {
            tile = d switch
            {
                E => tile.East(),
                W => tile.West(),
                NE => tile.NorthEast(),
                NW => tile.NorthWest(),
                SE => tile.SouthEast(),
                SW => tile.SouthWest(),
                _ => throw new()
            };
        }

        return tile;
    }

    static IEnumerable<Direction> GetDirections(string line)
    {
        var sb = new StringBuilder();
        foreach (var c in line)
        {
            sb.Append(c);
            Direction? d = sb.ToString() switch
            {
                "se" => SE,
                "ne" => NE,
                "sw" => SW,
                "nw" => NW,
                "e" => E,
                "w" => W,
                "s" or "n" => null,
                _ => throw new()
            };
            if (d.HasValue)
            {
                yield return d.Value;
                sb.Clear();
            }
        }

    }
}