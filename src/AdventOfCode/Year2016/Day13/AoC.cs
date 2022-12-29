namespace AdventOfCode.Year2016.Day13;


public class AoC201613
{
    public int Part1() => BFS((1, 1), (31, 39), null);
    public int Part2() => BFS((1, 1), null, 50);

    private int BFS(Coordinate start, Coordinate? target, int? max)
    {
        HashSet<Coordinate> visited = new()
        {
            start
        };
        Queue<(Coordinate pos, uint steps)> queue = new();
        queue.Enqueue((start, 0));

        while (queue.Any())
        {
            var (prev, steps) = queue.Dequeue();
            steps++;

            if (max.HasValue && steps > max)
                continue;

            var q = from next in prev.Neighbours()
                    where next.IsSpace && !visited.Contains(next)
                    select next;

            foreach (var next in q)
            {
                if (target.HasValue && next == target)
                {
                    return (int)steps;
                }
                queue.Enqueue((next, steps));
                visited.Add(next);
            }
        }
        return visited.Count;
    }
}

readonly record struct Coordinate(uint x, uint y)
{
    static readonly ulong input = ulong.Parse(Read.InputLines()[0]);

    public static implicit operator Coordinate((uint x, uint y) tuple) => new(tuple.x, tuple.y);
    public bool IsSpace => HammingWeight(x * x + 3 * x + 2 * x * y + y + y * y + input) % 2 == 0;
    static int HammingWeight(ulong i)
    {
        i = i - ((i >> 1) & 0x5555555555555555UL);
        i = (i & 0x3333333333333333UL) + ((i >> 2) & 0x3333333333333333UL);
        return (int)(unchecked(((i + (i >> 4)) & 0xF0F0F0F0F0F0F0FUL) * 0x101010101010101UL) >> 56);
    }
    public IEnumerable<Coordinate> Neighbours()
    {
        if (x > 0) yield return (x - 1, y);
        if (y > 0) yield return (x, y - 1);
        yield return (x + 1, y);
        yield return (x, y + 1);
    }
}
