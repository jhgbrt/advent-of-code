using Net.Code.Graph;
using Net.Code.Graph.Algorithms;

namespace AdventOfCode.Year2019.Day15;
public class AoC201915
{
    public AoC201915():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly IntCode machine;
    readonly Grid grid;
    Coordinate target;
    public AoC201915(string[] input, TextWriter writer)
    {
        this.writer = writer;
        var program = input.First().Split(',').Select(int.Parse).Select((n, i) => (n, i: i)).ToImmutableDictionary(x => x.i, x => x.n);
        machine = new IntCode(program);
        grid = new Grid();
        FillGrid(grid, Coordinate.Origin, Direction.N);
    }
    static readonly Direction[] directions = [Direction.N, Direction.S, Direction.W, Direction.E];
    public int Part1()
    {
        var vertices = grid.Keys;
        var edges = from vertex in grid.Keys
                    from direction in directions
                    let next = vertex + direction
                    where grid.Contains(next) && grid[next] != '#'
                    select Edge.Create(vertex, next, 1);

        var graph = GraphBuilder.Create<Coordinate, int>()
            .AddVertices(vertices)
            .AddEdges(edges)
            .BuildGraph();

        var result = Dijkstra.ShortestPaths(graph, Coordinate.Origin);
        return result.GetPath(target).Count();
    }
    void FillGrid(Grid grid, Coordinate pos, Direction direction)
    {

        foreach (var d in directions)
        {
            var next = pos + d;
            if (!grid.Contains(next))
            {
                var result = machine.Run((int)d) switch
                {
                    0 => '#',
                    1 => '.',
                    2 => 'O',
                };
                grid[next] = result;
                if (result != '#')
                {
                    if (result == 'O')
                    {
                        target = next;
                    }
                    FillGrid(grid, next, d);
                }
            }
        }
        if (pos != Coordinate.Origin)
        {
            var reverse = direction switch
            {
                Direction.N => Direction.S,
                Direction.E => Direction.W,
                Direction.S => Direction.N,
                Direction.W => Direction.E,
            };
            machine.Run((int)reverse);
            pos = pos + reverse;
        }
    }

    public int Part2() => Spread(grid, target, 0, 0);

    int Spread(Grid grid, Coordinate position, int time, int max)
    {
        var neighbours = from d in directions
                         let next = position + d
                         where grid[next] == '.'
                         select next;

        foreach (var n in neighbours)
        {
            grid[n] = 'O';
            max = Spread(grid, n, time + 1, max);
        }
        return Max(time, max);
    }
}

public class AoC201915Tests
{
    private readonly AoC201915 sut;
    public AoC201915Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC201915(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(266, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(274, sut.Part2());
    }
}

enum Direction : int { N = 1, E = 4, S = 2, W = 3}
readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate c, Direction d) => d switch
    {
        Direction.N => c with { y = c.y + 1 },
        Direction.E => c with { x = c.x + 1 },
        Direction.S => c with { y = c.y - 1 },
        Direction.W => c with { x = c.x - 1 },
    };
    public static Coordinate operator -(Coordinate c, Direction d) => c + d switch
    {
        Direction.N => Direction.S,
        Direction.E => Direction.W,
        Direction.S => Direction.N,
        Direction.W => Direction.E
    };
}
class Grid
{
    readonly Dictionary<Coordinate, char> items = [];
    readonly char unknown = 'U';
    public char this[Coordinate p]
    {
        get {
            return items.TryGetValue(p, out var c) ? c : unknown;
        }
        set {
            items[p] = value;
        }
    }

    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];
    public IEnumerable<Coordinate> Keys => items.Keys;
    private Coordinate topleft => new(items.Keys.Min(x => x.x), items.Keys.Min(x => x.y));
    private Coordinate bottomright => new(items.Keys.Max(x => x.x), items.Keys.Max(x => x.y));

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = topleft.y; y <= bottomright.y; y++)
        {
            for (int x = topleft.x; x <= bottomright.x; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

  
    public int Count() => items.Count;
    public bool Contains(Coordinate position) => items.ContainsKey(position);
}
class IntCode
{
    public bool IsTerminated { get; private set; }
    private Dictionary<int, int> program;
    public IntCode(ImmutableDictionary<int, int> program) => this.program = program.ToDictionary();

    public (int a, int b, int c) Step(int input) => (Run(input), Run(input), Run(input));

    public IEnumerable<int> Run()
    {
        while (!IsTerminated)
        {
            yield return Run(0);
        }
    }

    int index = 0;
    int offset = 0;

    public int Run(int input)
    {
        int opcode;
        while (!IsTerminated)
        {
            (opcode, var modes) = Decode(program[index]);

            if (opcode == 99)
                break;

            var parameters = opcode switch
            {
                1 or 2 or 7 or 8 => program.GetParameters(index, modes, 3),
                3 or 4 or 9 => program.GetParameters(index, modes, 1),
                5 or 6 => program.GetParameters(index, modes, 2)
            };

            var parameterValues = opcode switch
            {
                1 or 2 or 5 or 6 or 7 or 8 => program.GetValues(offset, parameters, 2),
                4 or 9 => program.GetValues(offset, parameters, 1),
                _ => Array.Empty<int>()
            };

            var parameterCount = parameters.Length;
            var jump = parameterCount + 1;

            switch (opcode)
            {
                case 1:
                {
                    var result = parameterValues.Sum();
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 2:
                {
                    var result = parameterValues.Product();
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 3:
                {
                    int result = input;
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 4:
                {
                    index += jump;
                    return parameterValues[0];
                }
                case 5:
                {
                    if (parameterValues[0] != 0) jump = (int)parameterValues[1] - index;
                }
                break;
                case 6:
                {
                    if (parameterValues[0] == 0) jump = (int)parameterValues[1] - index;
                }
                break;
                case 7:
                {
                    var result = parameterValues[0] < parameterValues[1] ? 1 : 0;
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 8:
                {
                    var result = parameterValues[0] == parameterValues[1] ? 1 : 0;
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 9:
                {
                    offset += (int)parameterValues[0];
                }
                break;
                default:
                    throw new Exception();
            }
            index += jump;
        }
        IsTerminated = true;
        return int.MinValue;
    }


    internal static (int opcode, IReadOnlyCollection<Mode> modes) Decode(int value)
    {
        Mode[] modes = new Mode[3];
        var opcode = value % 100;
        value /= 100;
        for (int i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return ((int)opcode, modes);
    }
}


static class Ex
{
    internal static Parameter[] GetParameters(this IReadOnlyDictionary<int, int> program, int index, IEnumerable<Mode> modes, int n)
        => Range(index + 1, n).Select(i => program.ContainsKey(i) ? program[i] : 0).Zip(modes, (l, r) => new Parameter(l, r)).ToArray();
    internal static void Set(this Dictionary<int, int> program, Parameter parameter, int offset, int value)
    {
        var index = parameter.mode switch
        {
            Mode.Relative => offset + parameter.index,
            _ => parameter.index
        };

        program[index] = value;
    }

    internal static int GetValue(this IReadOnlyDictionary<int, int> program, int offset, Parameter parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program.ContainsKey(index) ? program[index] : 0,
            Mode.Relative => program.ContainsKey(index + offset) ? program[index + offset] : 0,
            _ => throw new NotImplementedException()
        };
    }
    internal static int[] GetValues(this IReadOnlyDictionary<int, int> program, int relativeBase, IEnumerable<Parameter> parameters, int n)
        => parameters.Take(n).Select(p => program.GetValue(relativeBase, p)).ToArray();

}

record struct Parameter(int index, Mode mode);
enum Mode
{
    Position = 0,
    Immediate = 1,
    Relative = 2
}
