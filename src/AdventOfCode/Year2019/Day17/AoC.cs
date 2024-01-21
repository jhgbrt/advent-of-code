
namespace AdventOfCode.Year2019.Day17;
public partial class AoC201917
{
    public AoC201917() : this(Read.InputLines(), Console.Out) { }
    readonly TextWriter? writer;
    Grid grid;
    string[]? input;

    public AoC201917(string[] input, TextWriter? writer)
    {
        this.input = input;
        this.writer = writer;
        this.grid = CreateGrid();
    }

    internal AoC201917(Grid grid, TextWriter? writer)
    {
        this.grid = grid;
        this.writer = writer;
    }
    internal void Print() => writer?.WriteLine(CreateGrid());

    IntCode GetIntCode(int? n)
    {
        var program = input![0].Split(',').Select(long.Parse).ToArray();
        if (n.HasValue) program[0] = n.Value;
        var intcode = new IntCode(program);
        return intcode;
    }


    Grid CreateGrid()
    {
        var intcode = GetIntCode(null);
        var sb = new StringBuilder();
        foreach (var c in intcode.Run().Select(i => (char)i)) sb.Append(c);
        var lines = Read.String(sb.ToString());
        return new Grid(lines);
    }

    public int Part1() => (from c in grid.Points()
                           let n = grid.Neighbours(c)
                           where grid[c] == '#'
                           && n.All(n => grid[n] == '#')
                           select c.x * c.y).Sum();

    public long Part2()
    {
        var grid = CreateGrid();

        var position = grid.Find('^', '>', 'v', '<');
        var orientation = (Direction)grid[position];
        char? turn = null;
        List<Instruction> instructions = [];
        while (true)
        {
            int n = 0;
            var next = position + orientation;
            while (grid.Contains(next) && grid[next] == '#')
            {
                n++;
                position = next;
                next = position + orientation;
            }
            if (n > 0)
            {
                instructions.Add(new(turn, n));
            }

            var (left, right) = (position + orientation.Left(), position + orientation.Right());

            if (grid[right] == '#')
            {
                orientation = orientation.Right();
                turn = 'R';
            }
            else if (grid[left] == '#')
            {
                orientation = orientation.Left();
                turn = 'L';
            }
            else
            {
                break;
            }
        }
      
        var path = string.Join(",", instructions.Select(i => i.ToString())) + ",";

        var regex = new Regex(@"^(.{1,20})\1*(.{1,20})(?:\1|\2)*(.{1,20})(?:\1|\2|\3)*$");
        var match = regex.Match(path);
        var (A, B, C) = (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);


        var main = path.Replace(A, "A,").Replace(B, "B,").Replace(C, "C,").TrimEnd(',');

        var icinput = $"{main}\n{A.TrimEnd(',')}\n{B.TrimEnd(',')}\n{C.TrimEnd(',')}\nn\n".Select(c => (long)c).ToArray();
        
        var intcode = GetIntCode(2);

        return intcode.Run(icinput).Last();

    }


}

readonly record struct Instruction(char? Turn, int? Move)
{
    public override string ToString() => $"{Turn},{Move}";
}

public class AoC201917Tests
{
    private readonly ITestOutputHelper output;

    public AoC201917Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
        var input = Read.SampleLines();
        var sut = new AoC201917(input, null);
    }

    [Fact]
    public void TestSample()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC201917(new Grid(input), null);
        Assert.Equal(76, sut.Part1());
    }
    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines();
        var sut = new AoC201917(input, null);
        Assert.Equal(4800, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines();
        var sut = new AoC201917(input, null);//, new TestWriter(output));

        Assert.Equal(982279, sut.Part2());

    }


}


enum Direction : byte
{
    N = (byte)'^', E = (byte)'>', S = (byte)'v', W = (byte)'<'
}
static class DirectionEx
{
    public static Direction Left(this Direction direction) => direction switch
    {
        Direction.N => Direction.W,
        Direction.W => Direction.S,
        Direction.S => Direction.E,
        Direction.E => Direction.N
    };
    public static Direction Right(this Direction direction) => direction switch
    {
        Direction.N => Direction.E,
        Direction.E => Direction.S,
        Direction.S => Direction.W,
        Direction.W => Direction.N
    };
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";


    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
    public static Coordinate operator +(Coordinate c, Direction d) => d switch
    {
        Direction.N => c + (0, -1),
        Direction.E => c + (1, 0),
        Direction.S => c + (0, 1),
        Direction.W => c + (-1, 0),
    };
}

class Grid
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.y;
    public int Width => endmarker.x;
    public Grid(string[] input, char empty = '.')
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {
    }
    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    internal Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }

    public Coordinate Find(params char[] chars) => items.Where(i => chars.Any(c => i.Value == c)).First().Key;
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;
    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, endmarker.y)
        from x in Range(origin.x, endmarker.x)
        select new Coordinate(x, y);

    public IEnumerable<Coordinate> Neighbours(Coordinate p)
    {
        return
            from d in new (Direction direction, (int x, int y) delta)[]
            {
                (Direction.W, (-1, 0)),
                (Direction.S, (0, 1)),
                (Direction.E, (1, 0)),
                (Direction.N, (0, -1))
            }
            where IsValid(p + d.delta)
            select p + d.delta;
    }

    bool IsValid(Coordinate p) => p.x >= 0 && p.y >= 0 && p.x < endmarker.x && p.y < endmarker.y;

    public IEnumerable<Coordinate> BoundingBox(Coordinate p, int length)
    {
        return
            from x in Range(p.x - 1, length + 2)
            from y in new[] { p.y - 1, p.y, p.y + 1 }
            where x >= 0 && y >= 0
            && x < endmarker.x
            && y < endmarker.y
            select new Coordinate(x, y);
    }

    public IEnumerable<Coordinate> InteriorPoints() =>
        from y in Range(origin.y + 1, endmarker.y - 2)
        from x in Range(origin.x + 1, endmarker.x - 2)
        select new Coordinate(x, y);

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

    public bool Contains(Coordinate c) => IsValid(c);

}


