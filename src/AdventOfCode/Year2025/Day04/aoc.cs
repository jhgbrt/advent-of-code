namespace AdventOfCode.Year2025.Day04;

public class AoC202504(string[] input, TextWriter writer)
{
    Grid grid = new(input);
    public AoC202504() : this(Read.InputLines(), Console.Out) {}

    private bool CanBeRemoved(Coordinate pos) => grid[pos] == '@' && grid.CountNeighbours(pos) < 4;

    public int Part1() => grid.Keys.Count(CanBeRemoved);
    public int Part2()
    {
        var count = 0;
        List<Coordinate> toRemove;
        while ((toRemove = grid.FindAll(CanBeRemoved)).Any())
        {
            foreach (var p in toRemove)
            {
                grid[p] = '.';
                count++;
            }
        }

        return count;
    }
}

public class AoC202504Tests
{
    private readonly AoC202504 sut;
    public AoC202504Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202504(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(13, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(43, sut.Part2());
    }
}
readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

class Grid
{
    readonly char[,] items;
    readonly char empty = '.';
    public readonly int Height;
    public readonly int Width;

    public IEnumerable<Coordinate> Keys
    {
        get {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    yield return new Coordinate(x, y);
        }
    }

    public Grid(string[] input, char empty = '.')
    {
        this.empty = empty;
        Height = input.Length;
        Width = input[0].Length;
        items = new char[Height, Width];
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                items[y, x] = input[y][x];
            }
        }
    }

    public char this[Coordinate p]
    {
        get => items[p.y, p.x];
        set => items[p.y, p.x] = value;
    }

    public int CountNeighbours(Coordinate p)
    {
        Span<(int dx, int dy)> deltas = [(0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)];
        var (x, y) = p;
        int count = 0;
        foreach (var (dx, dy) in deltas)
        {
            var (nx, ny) = (x + dx, y + dy);
            if (nx >= 0 && ny >= 0 && nx < Width && ny < Height && this[new Coordinate(nx, ny)] != empty)
                count++;
        }
        return count;
    }

    public List<Coordinate> FindAll(Func<Coordinate, bool> predicate)
    {
        var result = new List<Coordinate>();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                var coord = new Coordinate(x, y);
                if (predicate(coord))
                    result.Add(coord);
            }
        }
        return result;
    }
  
}