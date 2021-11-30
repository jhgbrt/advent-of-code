namespace AdventOfCode.Year2020.Day16;

static class Ex
{

    internal static IEnumerable<string> ReadWhile(this StreamReader sr, Func<string, bool> predicate)
    {
        for (string? line = sr.ReadLine(); line != null && predicate(line); line = sr.ReadLine())
            yield return line;
    }
}