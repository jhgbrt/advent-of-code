namespace AdventOfCode.Year2017.Day16;

public class AoC201716
{
    static Dancer dancer = new Dancer(new StreamReader(Read.InputStream()));
    static string initial = "abcdefghijklmnop";
    public object Part1() => dancer.Run(initial);
    public object Part2() => dancer.Run(initial, 1000000000);

}

public class Dancer
{
    private readonly IReadOnlyList<Func<string, string>> _instructions;

    public Dancer(TextReader source) => _instructions = source.ReadInstructions().Select(Parse).ToList();

    private static Func<string, string> Parse(string instruction)
    {
        var data = instruction.Substring(1).Split('/');
        switch (instruction[0])
        {
            case 's':
                {
                    var x = Convert.ToInt32(data[0]);
                    return s => s.Spin(x);
                }
            case 'x':
                {
                    var x = Convert.ToInt32(data[0]);
                    var y = Convert.ToInt32(data[1]);
                    return s => s.Exchange(x, y);
                }
            case 'p':
                {
                    var x = data[0][0];
                    var y = data[1][0];
                    return s => s.Partner(x, y);
                }
            default: throw new InvalidOperationException();
        }
    }

    public string Run(string input, int rounds = 1)
    {
        var tmp = input;
        for (int i = 0; i < rounds; i++)
        {
            tmp = _instructions.Aggregate(tmp, (s, instruction) => instruction(s));
            if (tmp == input)
            {
                return Run(input, rounds % (i + 1));
            }
        }
        return tmp;
    }
}

static class Instructions
{
    public static string Spin(this string input, int n)
        => $"{input.Substring(input.Length - n, n)}{input.Substring(0, input.Length - n)}";

    public static string Exchange(this string input, int i, int j)
        => new StringBuilder(input) { [i] = input[j], [j] = input[i] }.ToString();

    public static string Partner(this string input, char a, char b)
        => Exchange(input, input.IndexOf(a), input.IndexOf(b));

    public static IEnumerable<string> ReadInstructions(this TextReader reader)
    {
        var sb = new StringBuilder();
        while (reader.Peek() >= 0)
        {
            var c = (char)reader.Read();
            if (c == ',')
            {
                yield return sb.ToString();
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }
        if (sb.Length > 0) yield return sb.ToString();
    }
}
