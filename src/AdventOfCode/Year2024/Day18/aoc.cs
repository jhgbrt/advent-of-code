using Net.Code.Graph;
using Net.Code.Graph.Algorithms;

namespace AdventOfCode.Year2024.Day18;

public class AoC202418(string[] input, int size, int n)
{
    public AoC202418() : this(Read.InputLines(), 71, 1024) { }
    Coordinate[] coordinates = (
        from line in input
        let comma = line.IndexOf(',')
        let x = int.Parse(line[..comma])
        let y = int.Parse(line[(comma + 1)..])
        select new Coordinate(x, y)).ToArray();

    public int Part1()
    {
        return FindShortestPaths(coordinates[..n].ToHashSet()).Count();
    }
    public string Part2()
    {
        var (lower, upper) = (n, coordinates.Length);
        var hashSet = new HashSet<Coordinate>();
        while (lower < upper)
        {
            var n = (lower + upper) / 2;
            hashSet.Clear();
            foreach (var c in coordinates[..n])
                hashSet.Add(c);
            var shortestPath = FindShortestPaths(hashSet);
            if (shortestPath.Any())
                lower = n + 1;
            else
                upper = n;
        }

        return coordinates[lower - 1].ToString();
    }
    IEnumerable<Coordinate> FindShortestPaths(HashSet<Coordinate> walls)
    {
        var g = GraphBuilder.Create<Coordinate, int>().AddEdges(
            from c in Coordinate.Range(size, size)
            where !walls.Contains(c)
            from n in c.Neighbours(size, size)
            where n.In(size, size) && !walls.Contains(n)
            select Edge.Create(c, n, 1)).BuildGraph();
        return Dijkstra.ShortestPaths(g, new(0, 0)).GetPath(new(size - 1, size - 1));
    }

}

public class AoC202418Tests
{
    private readonly AoC202418 sut;
    public AoC202418Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202418(input, 7, 12);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(22, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal("18,62", sut.Part2());
    }
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"{x},{y}";
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
    static readonly (int dx, int dy)[] deltas =
    {
        (0, -1),
        (1, 0),
        (0, 1),
        (-1, 0)
    };
    public IEnumerable<Coordinate> Neighbours(int width, int height)
    {
        var c = this;
        foreach (var d in deltas)
        {
            var n = c + d;
            if (n.In(width, height))
                yield return n;
        }
    }

    public static IEnumerable<Coordinate> Range(int width, int height)
    {
        for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                yield return new Coordinate(x, y);
    }

    public bool In(int width, int height) => (x, y, width - x, width - y) is ( >= 0, >= 0, > 0, > 0);
}