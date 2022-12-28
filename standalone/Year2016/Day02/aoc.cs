var input = File.ReadAllLines("input.txt").Where(l => !string.IsNullOrEmpty(l)).ToArray();
var sw = Stopwatch.StartNew();
var part1 = GetCode(input, Keypad1);
var part2 = GetCode(input, Keypad2);
Console.WriteLine((part1, part2, sw.Elapsed));
string GetCode(string[] inputs, Keypad keypad)
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