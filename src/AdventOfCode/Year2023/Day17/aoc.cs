namespace AdventOfCode.Year2023.Day17;
public class AoC202317
{
    public AoC202317() : this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly Grid grid;
    public AoC202317(string[] input, TextWriter writer)
    {
        this.grid = new(input);
        this.writer = writer;
    }

    public object Part1() => Solve(grid, 0, 3);

    public object Part2() => Solve(grid, 4, 10);


    internal int Solve(Grid grid, int min, int max)
    {
        var target = grid.BottomRight;

        var queue = new PriorityQueue<Vector, int>();
        queue.Enqueue(new Vector(Coordinate.Origin, Direction.E, 0), 0);
        queue.Enqueue(new Vector(Coordinate.Origin, Direction.S, 0), 0);

        var seen = new HashSet<Vector>();
        while (queue.TryDequeue(out var vector, out var heat))
        {
            if (vector.pos == target)
            {
                return heat;
            }
            var q = from next in Moves(vector, min, max)
                    where grid.Contains(next) && !seen.Contains(next)
                    select next;
            foreach (var next in q)
            {
                seen.Add(next);
                queue.Enqueue(next, heat + grid[next.pos]);
            }
        }
        throw new Exception();
    }
    internal IEnumerable<Vector> Moves(Vector vector, int min, int max)
    {
        if (vector.steps < max)
        {
            yield return vector.Move();
        }

        if (vector.steps >= min)
        {
            yield return vector.Left().Move();
            yield return vector.Right().Move();
        }
    }
}

public class AoC202317Tests
{
    private readonly AoC202317 sut;
    public AoC202317Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202317(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        var input = Read.SampleLines();
        var grid = new Grid(input);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(102, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(94, sut.Part2());
    }

    [Fact]
    public void AdvanceTest1()
    {
        var input = "12\n34";
        var grid = new Grid(Read.String(input));
        var vector = new Vector(Coordinate.Origin, Direction.E, 0);
        var result = sut.Moves(vector, 0, 3).Where(v => grid.Contains(v.pos));
        Assert.Equal(new[] { new Vector(new(1, 0), Direction.E, 1), new Vector(new(0, 1), Direction.S, 1) }, result);
    }
    [Fact]
    public void AdvanceTest2()
    {
        var input = "123\n456\n789";
        var grid = new Grid(Read.String(input));
        var vector = new Vector(new(0, 1), Direction.E, 0);
        var result = sut.Moves(vector, 0, 3).Where(v => grid.Contains(v.pos));
        Assert.Equal(new[] { new Vector(new(1, 1), Direction.E, 1), new Vector(new(0, 0), Direction.N, 1), new Vector(new(0, 2), Direction.S, 1) }, result);
    }
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    public void AdvanceTest3(int steps, int expectedsteps)
    {
        var input = "12";
        var grid = new Grid(Read.String(input));
        var vector = new Vector(Coordinate.Origin, Direction.E, steps);
        var result = sut.Moves(vector, 0, 3).Where(v => grid.Contains(v.pos));
        Assert.Equal(new[] { new Vector(new(1, 0), Direction.E, expectedsteps) }, result);
    }

    [Fact]
    public void AdvanceTest4()
    {
        var vector = new Vector(Coordinate.Origin, Direction.E, 3);
        var result = sut.Moves(vector, 0, 3);
        Assert.Equal(new[]{new Vector(new(0, -1), Direction.N, 1), new Vector(new(0, 1), Direction.S, 1) }, result);
    }
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    [InlineData(5, 6)]
    [InlineData(9, 10)]
    public void AdvanceTest5(int steps, int expectedsteps)
    {
        var input = "12";
        var grid = new Grid(Read.String(input));
        var vector = new Vector(Coordinate.Origin, Direction.E, steps);
        var result = sut.Moves(vector, 4, 10).Where(v => grid.Contains(v.pos));
        Assert.Equal(new[] { new Vector(new(1, 0), Direction.E, expectedsteps) }, result);
    }
    [Fact]
    public void AdvanceTest6()
    {
        var vector = new Vector(Coordinate.Origin, Direction.E, 10);
        var result = sut.Moves(vector, 4, 10);
        Assert.Equal(new[] { new Vector(new(0, -1), Direction.N, 1), new Vector(new(0, 1), Direction.S, 1) }, result);
    }
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 2)]
    [InlineData(2, 3)]
    [InlineData(3, 4)]
    public void AdvanceTest7(int steps, int expectedsteps)
    {
        var vector = new Vector(new(0,1), Direction.E, steps);
        var result = sut.Moves(vector, 4, 10);
        Assert.Equal(new[] { new Vector(new(1, 1), Direction.E, expectedsteps) }, result);
    }
    [Theory]
    [InlineData(5, 6)]
    [InlineData(9, 10)]
    public void AdvanceTest8(int steps, int expectedsteps)
    {
        var vector = new Vector(new(0, 1), Direction.E, steps);
        var result = sut.Moves(vector, 4, 10);
        Assert.Equal(new[] { new Vector(new(1, 1), Direction.E, expectedsteps), new Vector(new(0, 0), Direction.N, 1), new Vector(new(0, 2), Direction.S, 1) }, result);
    }
}

enum Direction { N, E, S, W }
class Grid
{
    readonly ImmutableDictionary<Coordinate, int> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    public Coordinate BottomRight => new(Width - 1, Height - 1);
    public int Height => endmarker.y;
    public int Width => endmarker.x;
    public Grid(string[] input)
    {
        items = (from y in Range(0, input.Length)
                 from x in Range(0, input[y].Length)
                 select (x, y, c: input[y][x] - '0')
                 ).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);
        endmarker = new(input[0].Length, input.Length);
    }


    private Grid(ImmutableDictionary<Coordinate, int> items, Coordinate endmarker)
    {
        this.items = items;
        this.endmarker = endmarker;
    }

    public int this[Coordinate p] => items[p];
    public int this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public int this[int x, int y] => this[new Coordinate(x, y)];


    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < endmarker.y; y++)
        {
            for (int x = origin.x; x < endmarker.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public bool Contains(Coordinate c) => items.ContainsKey(c);
    public bool Contains(Vector v) => items.ContainsKey(v.pos);

}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";
    public static Coordinate operator +(Coordinate c, Direction d) => d switch
    {
        Direction.N => c with { y = c.y - 1 },
        Direction.E => c with { x = c.x + 1 },
        Direction.S => c with { y = c.y + 1 },
        Direction.W => c with { x = c.x - 1 },
    };
}

readonly record struct Vector(Coordinate pos, Direction d, int steps)
{
    public override string ToString() => $"{pos},{d},{steps}";
    public Vector Move() => this with { pos = pos + d, steps = steps + 1 };
    public Vector Left() => this with
    {
        d = d switch
        {
            Direction.N => Direction.W,
            Direction.W => Direction.S,
            Direction.S => Direction.E,
            Direction.E => Direction.N
        },
        steps = 0
    };
    public Vector Right() => this with
    {
        d = d switch
        {
            Direction.N => Direction.E,
            Direction.E => Direction.S,
            Direction.S => Direction.W,
            Direction.W => Direction.N
        },
        steps = 0
    };
}
