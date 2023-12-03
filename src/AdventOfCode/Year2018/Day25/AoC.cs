namespace AdventOfCode.Year2018.Day25;

public class AoC201825
{
    static string[] input = Read.InputLines();
    static Point[] points = (
        from line in input
        let ints = (int[])MyConvert.ChangeType(line, typeof(int[]), new CsvLineFormatInfo())
        select new Point(ints[0], ints[1], ints[2], ints[3])
        ).ToArray();

    public int Part1()
    {
        var remaining = points.ToHashSet();
        List<HashSet<Point>> constellations = new();

        while (remaining.Any())
        {
            var first = remaining.First();
            var constellation = new HashSet<Point>
            {
                first
            };
            remaining.Remove(first);

            bool found = false;
            do
            {
                found = false;
                foreach (var r in remaining.ToList())
                {
                    if (constellation.Any(p => p.ManhattanDistance(r) <= 3))
                    {
                        constellation.Add(r);
                        remaining.Remove(r);
                        found = true;
                    }
                }
            } while (found);
            constellations.Add(constellation);
        }

        return constellations.Count;
    }

    public object Part2() => "";
}

readonly record struct Point(int x, int y, int z, int w)
{
    public int ManhattanDistance(Point other)
        => Abs(x - other.x) + Abs(y - other.y) + Abs(z - other.z) + Abs(w - other.w);
}
