var lines = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = lines.Sum(l => l.Length) - lines.Sum(CountChars);
var part2 = lines.Sum(CountEscaped) - lines.Sum(l => l.Length);
Console.WriteLine((part1, part2, sw.Elapsed));
int CountChars(string s)
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

int CountEscaped(string s) => s.Aggregate(2, (n, c) => c switch
{
    '"' or '\\' => n + 2,
    _ => n + 1
});
partial class AoCRegex
{
}

enum State
{
    None,
    Escaping
}