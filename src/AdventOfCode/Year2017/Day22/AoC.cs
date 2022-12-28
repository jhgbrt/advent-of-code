namespace AdventOfCode.Year2017.Day22;

public class AoC201722
{
    static string[] input = Read.InputLines().ToArray();
    public object Part1()
    {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid(10000);
    }

    public object Part2()
    {
        var grid = input.ToRectangular();
        return new Grid(grid).InfectGrid2(10000000);
    }

}

class Grid
{
    enum Direction
    {
        Up, Down, Left, Right
    }

    enum NodeState
    {
        Clean, Infected, Weakened, Flagged
    }

    private readonly IDictionary<(int row, int col), NodeState> _infections;
    private readonly (int row, int col) _start;
    private int _nofinfections;

    public Grid(char[,] grid)
    {
        _start = (row: grid.GetUpperBound(0) / 2, col: grid.GetUpperBound(1) / 2);

        _infections = grid
            .Enumerate()
            .Where(x => x.item == '#')
            .ToDictionary(x => (x.row, x.col), x => NodeState.Infected);

    }

    static (int row, int col) Step(int row, int col, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return (--row, col);
            case Direction.Down: return (++row, col);
            case Direction.Left: return (row, --col);
            case Direction.Right: return (row, ++col);
            default: throw new ArgumentOutOfRangeException();
        }
    }

    static Direction TurnLeft(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return Direction.Left;
            case Direction.Down: return Direction.Right;
            case Direction.Left: return Direction.Down;
            case Direction.Right: return Direction.Up;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    static Direction TurnRight(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return Direction.Right;
            case Direction.Down: return Direction.Left;
            case Direction.Left: return Direction.Up;
            case Direction.Right: return Direction.Down;
            default: throw new ArgumentOutOfRangeException();
        }
    }
    static Direction Reverse(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: throw new ArgumentOutOfRangeException();
        }
    }

    (Direction direction, (int row, int col) position) Visit1(Direction direction, (int row, int col) position)
    {
        var state = NodeState.Clean;
        if (_infections.TryGetValue(position, out var tmp))
        {
            state = tmp;
        }
        switch (state)
        {
            case NodeState.Clean:
                _nofinfections++;
                state = NodeState.Infected;
                direction = TurnLeft(direction);
                break;
            case NodeState.Infected:
                state = NodeState.Clean;
                direction = TurnRight(direction);
                break;
        }
        _infections[position] = state;
        position = Step(position.row, position.col, direction);
        return (direction, position);
    }
    (Direction direction, (int row, int col) position) Visit2(Direction direction, (int row, int col) position)
    {
        var state = NodeState.Clean;
        if (_infections.TryGetValue(position, out var tmp))
        {
            state = tmp;
        }
        switch (state)
        {
            case NodeState.Clean:
                state = NodeState.Weakened;
                direction = TurnLeft(direction);
                break;
            case NodeState.Infected:
                state = NodeState.Flagged;
                direction = TurnRight(direction);
                break;
            case NodeState.Weakened:
                _nofinfections++;
                state = NodeState.Infected;
                break;
            case NodeState.Flagged:
                state = NodeState.Clean;
                direction = Reverse(direction);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _infections[position] = state;
        position = Step(position.row, position.col, direction);
        return (direction, position);
    }

    public int InfectGrid(int iterations)
    {
        var direction = Direction.Up;
        var position = _start;

        for (int i = 0; i < iterations; i++)
        {
            (direction, position) = Visit1(direction, position);
        }

        return _nofinfections;
    }

    public int InfectGrid2(int iterations)
    {
        var direction = Direction.Up;
        var position = _start;

        for (int i = 0; i < iterations; i++)
        {
            (direction, position) = Visit2(direction, position);
        }

        return _nofinfections;
    }
}

static class Extensions
{
    public static IEnumerable<string> ReadLines(this string s)
    {
        using (var reader = new StringReader(s))
        {
            foreach (var line in reader.ReadLines()) yield return line;
        }
    }

    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.Peek() >= 0) yield return reader.ReadLine()!;
    }

    public static char[,] ToRectangular(this IEnumerable<string> lines)
        => lines.Select(s => s.ToCharArray()).ToArray().ToRectangular();

    public static string[] FromRectangular(this char[,] array)
        => array.ToJagged().Select(s => new string(s)).ToArray();

    static T[][] ToJagged<T>(this T[,] input)
    {
        var rows = input.GetUpperBound(0) + 1;
        var cols = input.GetUpperBound(1) + 1;
        var result = new T[rows][];
        for (var i = 0; i < rows; i++)
        {
            result[i] = new T[cols];
            for (var j = 0; j < cols; j++)
            {
                result[i][j] = input[i, j];
            }
        }
        return result;
    }

    static T[,] ToRectangular<T>(this T[][] arrays)
    {
        int length = arrays.Max(a => a.Length);
        T[,] ret = new T[arrays.Length, length];
        for (int i = 0; i < arrays.Length; i++)
        {
            var array = arrays[i];
            for (int j = 0; j < arrays[i].Length; j++)
            {
                ret[i, j] = array[j];
            }
        }
        return ret;
    }

    public static IEnumerable<(int row, int col, T item)> Enumerate<T>(this T[,] input)
    {
        var rows = input.GetUpperBound(0) + 1;
        var cols = input.GetUpperBound(1) + 1;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                yield return (row, col, input[row, col]);
            }
        }

    }
}

