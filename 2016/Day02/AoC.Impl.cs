namespace AdventOfCode.Year2016.Day02;

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt").Where(l => !string.IsNullOrEmpty(l)).ToArray();

    internal static Result Part1() => Run(() => GetCode(input, keypad1));
    internal static Result Part2() => Run(() => GetCode(input, keypad2));

    internal static readonly Keypad keypad1 = new(new char?[,] {
                { '1', '2', '3'},
                { '4', '5', '6'},
                { '7', '8', '9'},
            }, new Coordinate(1, 1));

    internal static readonly Keypad keypad2 = new(new char?[,] {
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
