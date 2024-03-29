namespace AdventOfCode.Year2016.Day02;

public class AoC201602
{
    public static string[] input = Read.InputLines().Where(l => !string.IsNullOrEmpty(l)).ToArray();

    public object Part1() => GetCode(input, Keypad1);
    public object Part2() => GetCode(input, Keypad2);

    internal static Keypad Keypad1 => new(new char?[,] {
                { '1', '2', '3'},
                { '4', '5', '6'},
                { '7', '8', '9'},
            }, new Coordinate(1, 1));

    internal static Keypad Keypad2 => new(new char?[,] {
                { null, null, '1', null, null},
                { null, '2', '3', '4', null},
                { '5', '6', '7', '8', '9'},
                { null, 'A', 'B', 'C', null},
                { null, null, 'D', null, null},
            }, new Coordinate(2, 0));

    internal static string GetCode(string[] inputs, Keypad keypad)
    {
        var sb = new StringBuilder();
        foreach (var input in inputs)
        {
            foreach (var c in input)
            {
                keypad.Move(c);
            }
            sb.Append(keypad.Current);
        }
        return sb.ToString();
    }
}
public record struct Coordinate(int Row, int Column)
{
    public Coordinate Move(char direction) => direction switch
    {
        'U' => this with { Row = Row - 1 },
        'D' => this with { Row = Row + 1 },
        'L' => this with { Column = Column - 1 },
        'R' => this with { Column = Column + 1 },
        _ => throw new InvalidOperationException(),
    };
}

public class Keypad
{
    char?[,] _keys;
    Coordinate _coordinate;

    public Keypad(char?[,] keys, Coordinate coordinate)
    {
        _keys = keys;
        _coordinate = coordinate;
    }

    public char? Current => _keys[_coordinate.Row, _coordinate.Column];

    public void Move(char direction)
    {
        var next = _coordinate.Move(direction);
        if (
            next.Row >= 0 && next.Row < _keys.GetLength(0)
            && next.Column >= 0 && next.Column < _keys.GetLength(1)
            && _keys[next.Row, next.Column].HasValue
            )
            _coordinate = next;
    }
}