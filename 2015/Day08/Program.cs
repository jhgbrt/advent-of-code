using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static string[] lines = File.ReadAllLines("input.txt");

    public static object Part1() => lines.Sum<string>(l => l.Length) - lines.Sum<string>(CountChars);
    public static object Part2() => lines.Sum<string>(CountEscaped) - lines.Sum<string>(l => l.Length);
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
    public void Test1() => Assert.Equal(1333, Part1());
    [Fact]
    public void Test2() => Assert.Equal(2046, Part2());
}


enum State
{
    None,
    Escaping
}
