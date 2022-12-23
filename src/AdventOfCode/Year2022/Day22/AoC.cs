using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day22;
using static Direction;
public class AoC202222
{
    static bool usesample = false;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    static Grid grid = Grid.Parse((usesample ? sample : input).TakeWhile(s => !string.IsNullOrEmpty(s)).ToArray());
    static string instructions = (usesample ? sample : input).SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(1).First();
    public object Part1()
    {
        var x = Range(0, grid.Width).First(x => grid[x, 0] != ' ');
        var v = new Vector(new Point(x, 0), E);

        //Console.Clear();
        //Console.WriteLine(v);
        foreach (var i in GetInstructions(instructions))
        {
            //Console.WriteLine(i);
            v = grid.Move(v, i);
            //grid.Draw();
            //Console.WriteLine(v);
            //Console.ReadLine();
            //Console.Clear();
        }
        Console.WriteLine(v);
        //Facing is 0 for right (>), 1 for down (v), 2 for left (<), and 3 for up (^).
        var result = (v.position.y + 1L) * 1000L + (v.position.x + 1L) * 4L + v.direction switch
        {
            E => 0, S => 1, W => 2, N => 3, _ => throw new Exception()
        };

        return result;
    }
    public object Part2() => "";


    private static IEnumerable<Instruction> GetInstructions(string path)
    {
        StringBuilder steps = new();
        StringBuilder rotation = new();
        for (int i = 0; i < path.Length; i++)
        {
            (steps, rotation) = path[i] switch
            {
                >= '0' and <= '9' => (steps.Append(path[i]), rotation),
                _ => (steps, rotation.Append(path[i]))
            };
            if (rotation.Length > 0)
            {
                yield return new Instruction(int.Parse(steps.ToString()), rotation[0]);
                steps.Clear();
                rotation.Clear();
            }
        }
    }

}


class Grid
{
    char[,] _grid;
    Dictionary<(int x, int y), char> _path = new();
    public Grid(char[,] grid) => _grid = grid;
    public char this[Point p] 
    { 
        get => InBounds(p) ? this[p.x, p.y] : ' '; 
        set 
        {
            if (InBounds(p)) this[p.x, p.y] = value; 
        }
    }
    public char this[int x, int y] { get => _grid[y, x]; set => _grid[y, x] = value; }
    public int Height => _grid.GetLength(0);
    public int Width => _grid.GetLength(1);

    public (Point lower, Point upper) Bound(Point p) => (new(MinX(p.y), MinY(p.x)), new(MaxX(p.y), MaxY(p.x)));

    int MinY(int x)
    {
        for (int y = 0; y < Height; y++)
            if (this[x, y] != ' ') return y;
        return Height;
    }
    int MaxY(int x)
    {
        for (int y = Height - 1; y >= 0; y--)
            if (this[x, y] != ' ') return y;
        return -1;
    }
    int MinX(int y)
    {
        for (int x = 0; x < Width; x++)
            if (this[x, y] != ' ') return x;
        return Width;
    }
    int MaxX(int y)
    {
        for (int x = Width - 1; x >= 0; x--)
            if (this[x, y] != ' ') return x;
        return -1;
    }

    public static Grid Parse(string[] lines)
    {
        var grid = new Grid(new char[lines.Length, lines.Max(l => l.Length)]);
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
                grid[x, y] = lines[y][x];
            for (var x = lines[y].Length; x < grid.Width; x++)
                grid[x, y] = ' ';
        }
        return grid;
    }
    public Vector Move(Vector v, Instruction i)
    {
        _path[(v.position.x, v.position.y)] = (char)v.direction;
        for (int j = 0; j < i.steps; j++)
        {
            var next = v.Move(Bound(v.position));
            if (this[next.position] == '.')
            {
                v = next;
                _path[(v.position.x, v.position.y)] = (char)v.direction;
            }
            else
                break;
        }
        v = v.Rotate(i.rotation);
        _path[(v.position.x, v.position.y)] = (char) v.direction;
        return v;
    }
   
    private bool InBounds(Point p) => p.x >= MinX(p.y) && p.x <= MaxX(p.y) && p.y >= MinY(p.x) && p.y <= MaxY(p.x);

    public void Draw()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (!_path.TryGetValue((x, y), out var c))
                    c = this[x, y];
                Console.ForegroundColor = c switch
                {
                    '>' or '<' or 'v' or '^' => ConsoleColor.Yellow,
                    '#' => ConsoleColor.White,
                    _ => ConsoleColor.Gray
                };
                Console.Write(c);
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                sb.Append(_path.TryGetValue((x, y), out var c) ? c : this[x, y]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

record Instruction(int steps, char rotation) 
{
    public override string ToString() => $"{steps}{rotation}";
}
enum Direction { N = '^', E = '>', S = 'v', W = '<' }
record Vector(Point position, Direction direction)
{
    public override string ToString() => $"{position}{direction}";

    public Vector Move((Point lower, Point upper) bounds)
    {
        return direction switch
        {
            N => this with { position = position.N(bounds.lower.y, bounds.upper.y) },
            E => this with { position = position.E(bounds.lower.x, bounds.upper.x) },
            S => this with { position = position.S(bounds.lower.y, bounds.upper.y) },
            W => this with { position = position.W(bounds.lower.x, bounds.upper.x) },
            _ => throw new NotSupportedException()
        };
    }

    public Vector Rotate(char rotation) => (rotation, direction) switch
    {
        ('R', N) => this with { direction = E },
        ('R', E) => this with { direction = S },
        ('R', S) => this with { direction = W },
        ('R', W) => this with { direction = N },
        ('L', N) => this with { direction = W },
        ('L', W) => this with { direction = S },
        ('L', S) => this with { direction = E },
        ('L', E) => this with { direction = N },
        _ => throw new NotSupportedException()
    };
}


readonly record struct Point(int x, int y)
{
    public override string ToString() => $"({x},{y})";
    public Point N(int min, int max) => this with { y = y == min ? max : y - 1 };
    public Point E(int min, int max) => this with { x = x == max ? min : x + 1 };
    public Point S(int min, int max) => this with { y = y == max ? min : y + 1 };
    public Point W(int min, int max) => this with { x = x == min ? max : x - 1 };
}
