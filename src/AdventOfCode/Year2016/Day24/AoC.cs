namespace AdventOfCode.Year2016.Day24;
public class AoC201624
{
    static readonly string[] input = Read.InputLines();
    static readonly IReadOnlyDictionary<Coordinate, char> grid = (from y in Range(0, input.Length)
                                         from x in Range(0, input[y].Length)
                                         select (c: new Coordinate(x, y), v: input[y][x])).ToDictionary(x => x.c, x => x.v);
    static readonly IReadOnlyList<Coordinate> locations = grid
        .Where(kv => char.IsDigit(kv.Value)).Select(kv => kv.Key)
        .ToList();

    static readonly Coordinate start = locations.Single(c => grid[c] == '0');
    static readonly int maxX = grid.Max(kv => kv.Key.x);
    static readonly int maxY = grid.Max(kv => kv.Key.y);

    static readonly  IReadOnlyDictionary<(Coordinate src, Coordinate dst), int> distances = (
        from src in locations
        from dst in locations
        where src != dst
        let distance = Distance(src, dst)
        select (src, dst, distance)
        ).ToDictionary(x => (x.src, x.dst), x => x.distance);

    public object Part1() => (
            from path in locations.Except(start).GetPermutations(locations.Count - 1)
            let total = (
                    from pair in start.Concat(path).Windowed2()
                    select distances[pair]
                    ).Sum()
            select total
            ).Min();
    public object Part2() => (
               from path in locations.Except(start).GetPermutations(locations.Count - 1)
               let total = (
                       from pair in start.Concat(path).Append(start).Windowed2()
                       select distances[pair]
                       ).Sum()
               select total
               ).Min();


    static int Distance(Coordinate from, Coordinate to)
    {
        var queue = new Queue<(Coordinate to, int distance)>();
        queue.Enqueue((from, distance: 0));
        var visited = new HashSet<Coordinate> { from };
        while (queue.Any())
        {
            var (current, distance) = queue.Dequeue();
            if (current == to)
            {
                return distance;
            }

            foreach (var n in from n in current.Neighbours(maxX, maxY)
                              where !visited.Contains(n) && grid[n] != '#'
                              select n)
            {
                queue.Enqueue((n, distance + 1));
                visited.Add(n);
            }
        }
        return -1;
    }
}


readonly record struct Coordinate(int x, int y)
{
    public IEnumerable<Coordinate> Neighbours(int maxX, int maxY)
    {
        if (x > 0) yield return this with { x = x - 1 };
        if (y > 0) yield return this with { y = y - 1 };
        if (x < maxX) yield return this with { x = x + 1 };
        if (y < maxY) yield return this with { y = y + 1 };
    }
}