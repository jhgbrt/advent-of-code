namespace AdventOfCode.Year2020.Day24;
static class Ex
{
    public static Tile ToTile(this string line)
    {
        var tile = new Tile(0, 0, 0);

        foreach (var d in GetDirections(line))
        {
            tile = d switch
            {
                Direction.E => tile.East(),
                Direction.W => tile.West(),
                Direction.NE => tile.NorthEast(),
                Direction.NW => tile.NorthWest(),
                Direction.SE => tile.SouthEast(),
                Direction.SW => tile.SouthWest(),
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
                "se" => Direction.SE,
                "ne" => Direction.NE,
                "sw" => Direction.SW,
                "nw" => Direction.NW,
                "e" => Direction.E,
                "w" => Direction.W,
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