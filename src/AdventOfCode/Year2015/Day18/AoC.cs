namespace AdventOfCode.Year2015.Day18;

public class AoC201518
{
    static string[] lines = Read.InputLines();
    static Grid grid = new Grid((
        from y in Range(0, lines.Length)
        from x in Range(0, lines[y].Length)
        where lines[y][x] == '#'
        select new Coordinate(x, y)
        ).ToHashSet(), lines.Length);

    public object Part1() => Range(0, 100).Aggregate(grid, (g, i) => g.Next1()).Count();
    public object Part2() => Range(0, 100).Aggregate(grid, (g, i) => g.Next2()).Count();
}
record Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}

class Grid
{
    HashSet<Coordinate> _lights;
    int _size;
    public Grid(HashSet<Coordinate> lights, int size)
    {
        _lights = lights;
        _size = size;
    }

    public Grid Next1() => new(Traverse().Where(NextShouldBeOn).ToHashSet(), _size);
    public Grid Next2() => new(Traverse().Where(c => c switch
    {
        (0 or 99, 0 or 99) => true,
        _ => NextShouldBeOn(c)
    }).ToHashSet(), _size);

    private bool NextShouldBeOn(Coordinate c) => IsOn(c) switch
    {
        true => Neighbours(c).Count(IsOn) switch { 2 or 3 => true, _ => false },
        false => Neighbours(c).Count(IsOn) switch { 3 => true, _ => false },
    };

    internal int Count() => _lights.Count();

    private bool IsOn(Coordinate coordinate) => _lights.Contains(coordinate);

    private IEnumerable<Coordinate> Traverse()
    {
        for (int y = 0; y < _size; y++) for (int x = 0; x < _size; x++) yield return new(x, y);
    }

    public IEnumerable<Coordinate> Neighbours(Coordinate c)
    {
        if (c.x > 0 && c.y > 0) yield return new(c.x - 1, c.y - 1);
        if (c.y > 0) yield return new(c.x, c.y - 1);
        if (c.x < _size - 1 && c.y > 0) yield return new(c.x + 1, c.y - 1);

        if (c.x > 0) yield return new(c.x - 1, c.y);
        if (c.x < _size - 1) yield return new(c.x + 1, c.y);

        if (c.x > 0 && c.y < _size - 1) yield return new(c.x - 1, c.y + 1);
        if (c.y < _size - 1) yield return new(c.x, c.y + 1);
        if (c.x < _size - 1 && c.y < _size - 1) yield return new(c.x + 1, c.y + 1);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < _size; y++)
        {
            for (int x = 0; x < _size; x++)
                sb.Append(_lights.Contains(new(x, y)) ? '#' : '.');
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
