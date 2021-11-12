using System.Text;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt").Where(l => !string.IsNullOrEmpty(l)).ToArray();

    internal static Result Part1() => Run(() => GetCode(input, keypad1));
    internal static Result Part2() => Run(() => GetCode(input, keypad2));

    internal static readonly Keypad keypad1 = new (new char?[,] {
                { '1', '2', '3'},
                { '4', '5', '6'},
                { '7', '8', '9'},
            }, new Coordinate(1, 1));

    internal static readonly Keypad keypad2 = new (new char?[,] {
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

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal("24862", Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal("46C91", Part2().Value);

    static string[] testinputs = new[] { "ULL", "RRDDD", "LURDL", "UUUUD" };

    [Fact]
    public void ExamplesPart1()
    {
        var code = AoC.GetCode(testinputs, AoC.keypad1);
        Assert.Equal("1985", code);
    }


    [Fact]
    public void ExamplesPart2()
    {
        var code = AoC.GetCode(testinputs, AoC.keypad2);
        Assert.Equal("5DB3", code);
    }

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
