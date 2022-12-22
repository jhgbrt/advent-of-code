using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day22;
using static Direction;
public class AoC202222
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    static Grid grid = Grid.Parse((usesample ? sample : input).TakeWhile(s => !string.IsNullOrEmpty(s)).ToArray());
    static string path = (usesample ? sample : input).SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(1).First();
    public object Part1()
    {
        //Console.WriteLine(grid);
        //Console.WriteLine(path);
        //foreach (var i in GetInstructions(path))
        //    Console.WriteLine(i);

        var x = Range(0, grid.Width).First(x => grid[x, 0] != ' ');
        var v = new Vector(new Point(x, 0), E);

        Console.WriteLine(v);
        foreach (var i in GetInstructions(path))
        {
            var bounds = grid.Bound(v.position);
            Console.WriteLine(i);
            Console.WriteLine(bounds);

            var steps = i.steps;
            for (int s = 0; s < steps; s++)
            {
                // todo check bounds and walls
                v = v.Move();
            }

            v = v.Rotate(i.rotation);
            Console.WriteLine(v);
        }

        var result = (v.position.x + 1) * 1000 + (v.position.y + 1) * 4 + v.direction switch
        {
            E => 0, S => 1, W => 2, N => 3
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
    public Grid(char[,] grid) => _grid = grid;
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

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }



}

record Instruction(int steps, char rotation);
enum Direction { N, E, S, W }
record Vector(Point position, Direction direction)
{
    public Vector Move()
    {
        return direction switch
        {
            N => this with { position = position + (0, -1) },
            E => this with { position = position + (1, 0) },
            S => this with { position = position + (0, 1) },
            W => this with { position = position + (-1, 0) },
            _ => throw new NotSupportedException()
        };
    }

    static T Mod<T>(T n, T m) where T : INumber<T> => (n % m + m) % m;

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

    public static Point operator +(Point left, (int x, int y) delta) => new(left.x + delta.x, left.y + delta.y);
}
