namespace AdventOfCode.Year2019.Day11;

public class AoC201911
{
    internal static string[] input = Read.InputLines();

    public object Part1()
    {
        var cpu = IntCode.Load(input[0]);

        var robot = new Robot(Coordinate.Origin, Facing.N);
        var grid = new Grid();
        var painted = new HashSet<Coordinate>();
        cpu.CurrentInput = 0;
        var output = cpu.Run();
        foreach (var (paint, turn) in output.Chunked2())
        {
            var value = paint == 0 ? '.' : '#';

            grid[robot.position] = value;

            if (paint == 1) painted.Add(robot.position);

            robot = turn switch
            {
                0 => robot.TurnLeft(),
                1 => robot.TurnRight()
            };

            robot = robot.Move();

            cpu.CurrentInput = grid[robot.position] == '.' ? 0 : 1;

            //Console.Clear();
            //Console.WriteLine(grid);

        }

        return painted.Count;
    }
    public object Part2()
    {
        var cpu = IntCode.Load(input[0]);

        var robot = new Robot(Coordinate.Origin, Facing.N);
        var grid = new Grid();
        var painted = new HashSet<Coordinate>();
        grid[Coordinate.Origin] = '#';
        cpu.CurrentInput = grid[robot.position] == '.' ? 0 : 1;
        var output = cpu.Run();
        foreach (var (paint, turn) in output.Chunked2())
        {
            var value = paint == 0 ? '.' : '#';

            grid[robot.position] = value;

            if (paint == 1) painted.Add(robot.position);

            robot = turn switch
            {
                0 => robot.TurnLeft(),
                1 => robot.TurnRight()
            };

            robot = robot.Move();

            cpu.CurrentInput = grid[robot.position] == '.' ? 0 : 1;
        }
        return AsciiFonts.GetFont(AsciiFontSize._4x6).Decode(grid.ToString());
    }

}

class IntCode
{
    public static IntCode Load(string input)
    {
        var program = input.Split(',').Select((s, i) => (s, i)).ToDictionary(x => (long)x.i, x => long.Parse(x.s));
        return new IntCode(program);
    }

    readonly Dictionary<long, long> program;
    int index;
    int offset;
    public IntCode(Dictionary<long, long> program) => this.program = program;

    public int CurrentInput;
    internal IEnumerable<long> Run()
    {
        int opcode;
        while (true)
        {
            (opcode, var modes) = Decode(program[index]);

            if (opcode == 99)
                break;

            var parameters = opcode switch
            {
                1 or 2 or 7 or 8 => GetParameters(index, modes, 3),
                3 or 4 or 9 => GetParameters(index, modes, 1),
                5 or 6 => GetParameters(index, modes, 2)
            };

            var parameterValues = opcode switch
            {
                1 or 2 or 5 or 6 or 7 or 8 => GetValues(offset, parameters, 2),
                4 or 9 => GetValues(offset, parameters, 1),
                _ => Array.Empty<long>()
            };

            var parameterCount = parameters.Length;
            var jump = parameterCount + 1;

            switch (opcode)
            {
                case 1:
                    {
                        var result = parameterValues.Sum();
                        Set(parameters[^1], offset, result);
                    }
                    break;
                case 2:
                    {
                        var result = parameterValues.Product();
                        Set(parameters[^1], offset, result);
                    }
                    break;
                case 3:
                    {
                        var result = CurrentInput;
                        Set(parameters[^1], offset, result);
                    }
                    break;
                case 4:
                    {
                        yield return parameterValues[0];
                    }
                    break;
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
                        Set(parameters[^1], offset, result);
                    }
                    break;
                case 8:
                    {
                        var result = parameterValues[0] == parameterValues[1] ? 1 : 0;
                        Set(parameters[^1], offset, result);
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
    }

    Parameter[] GetParameters(int index, IEnumerable<Mode> modes, int n)
        => Range(index + 1, n).Select(i => program.ContainsKey(i) ? program[i] : 0).Zip(modes, (l, r) => new Parameter(l, r)).ToArray();

    internal static (int opcode, IReadOnlyCollection<Mode> modes) Decode(long value)
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
    Dictionary<long, long> Set(Parameter parameter, long relativeBase, long value)
    {
        var index = parameter.mode switch
        {
            Mode.Relative => relativeBase + parameter.index,
            _ => parameter.index
        };

        program[index] = value;
        return program;
    }

    long GetValue(long relativeBase, Parameter parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program.ContainsKey(index) ? program[index] : 0,
            Mode.Relative => program.ContainsKey(index + relativeBase) ? program[index + relativeBase] : 0,
            _ => throw new NotImplementedException()
        };
    }

    long[] GetValues(int relativeBase, IEnumerable<Parameter> parameters, int n)
        => parameters.Take(n).Select(p => GetValue(relativeBase, p)).ToArray();
}
record struct Parameter(long index, Mode mode);
enum Mode
{
    Position = 0,
    Immediate = 1,
    Relative = 2
}

readonly record struct Robot(Coordinate position, Facing facing)
{
    public Robot TurnLeft() => this with
    {
        facing = facing switch
        {
            Facing.N => Facing.W,
            Facing.W => Facing.S,
            Facing.S => Facing.E,
            Facing.E => Facing.N
        }
    }; 
    public Robot TurnRight() => this with
    {
        facing = facing switch
        {
            Facing.N => Facing.E,
            Facing.E => Facing.S,
            Facing.S => Facing.W,
            Facing.W => Facing.N
        }
    };

    public Robot Move() => this with
    {
        position = facing switch
        {
            Facing.N => position.Up(),
            Facing.E => position.Right(),
            Facing.S => position.Down(),
            Facing.W => position.Left()
        }
    };
        
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public Coordinate Left() => this with { x = x - 1 };
    public Coordinate Right() => this with { x = x + 1 };
    public Coordinate Down() => this with { y = y + 1 };
    public Coordinate Up() => this with { y = y - 1 };
    public override string ToString() => $"({x},{y})";

}

enum Facing { N, E, S, W };

class Grid
{
    readonly Dictionary<Coordinate, char> items = new();
    readonly char empty = '.';
    public char this[Coordinate p]
    {
        get
        {
            return items.TryGetValue(p, out var c) ? c : empty;
        }
        set
        {
            if (value == empty)
            {
                items.Remove(p);
            }
            else
            {
                items[p] = value;
            }
        }
    }

    public char this[(int x, int y) p] => this[new Coordinate(p.x, p.y)];
    public char this[int x, int y] => this[new Coordinate(x, y)];

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
}
