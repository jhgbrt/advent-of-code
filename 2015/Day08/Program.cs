
using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] lines = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => lines.Sum<string>(l => l.Length) - lines.Sum<string>(CountChars));
    internal static Result Part2() => Run(() => lines.Sum<string>(CountEscaped) - lines.Sum<string>(l => l.Length));
    static int CountChars(string s)
    {
        var n = 0;

        var state = State.None;

        for (var i = 1; i < s.Length - 1; i++)
        {
            var c = s[i];

            (n, state, i) = (state, c) switch
            {
                (State.None, '\\') => (n, State.Escaping, i),
                (State.Escaping, '"') => (n + 1, State.None, i),
                (State.Escaping, '\\') => (n + 1, State.None, i),
                (State.Escaping, 'x') => (n + 1, State.None, i + 2),
                _ => (n + 1, State.None, i)
            };
        }

        return n;
    }

    static int CountEscaped(string s) => s.Aggregate(2, (n, c) => c switch { '"' or '\\' => n + 2, _ => n + 1 });
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1333, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(2046, Part2().Value);
}


enum State
{
    None,
    Escaping
}
