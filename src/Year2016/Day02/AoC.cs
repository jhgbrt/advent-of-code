namespace AdventOfCode.Year2016.Day02;

public class AoCImpl : AoCBase
{
    public static string[] input = Read.InputLines(typeof(AoCImpl)).Where(l => !string.IsNullOrEmpty(l)).ToArray();

    public override object Part1() => GetCode(input, Keypad1);
    public override object Part2() => GetCode(input, Keypad2);

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
