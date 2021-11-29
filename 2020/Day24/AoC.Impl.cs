namespace AdventOfCode.Year2020.Day24;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() =>
    {
        var tiles = from line in input select line.ToTile();

        var flippedTiles = tiles.Aggregate(
            ImmutableHashSet<Tile>.Empty,
            (set, tile) => set.Contains(tile) ? set.Remove(tile) : set.Add(tile)
            );

        return flippedTiles.Count;
    });
    internal static Result Part2() => Run(() =>
    {
        var tiles =
            from line in File.ReadLines("input.txt")
            select line.ToTile();

        var flippedTiles = tiles.Aggregate(
            ImmutableHashSet<Tile>.Empty,
            (set, tile) => set.Contains(tile) ? set.Remove(tile) : set.Add(tile)
            );

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
        return flippedTiles.Count;
    });

}

record Tile(int x, int y, int z)
{
    public Tile East() => this with { x = x + 1, y = y - 1 };
    public Tile West() => this with { x = x - 1, y = y + 1 };
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
