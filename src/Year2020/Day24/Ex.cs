using static AdventOfCode.Year2020.Day24.Direction;
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