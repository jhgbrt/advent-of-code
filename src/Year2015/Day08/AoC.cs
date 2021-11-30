namespace AdventOfCode.Year2015.Day08;

public class AoC201508 : AoCBase
{
    static string[] lines = Read.InputLines(typeof(AoC201508));

    public override object Part1() => lines.Sum<string>(l => l.Length) - lines.Sum<string>(CountChars);
    public override object Part2() => lines.Sum<string>(CountEscaped) - lines.Sum<string>(l => l.Length);
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


enum State
{
    None,
    Escaping
}

