namespace AdventOfCode.Year2018.Day07;

static class Ex
{
    public static (char from, char to) ToEdge(this string input) => (input[5], input[36]);
    public static Graph ToGraph(this IEnumerable<string> input) => new Graph(input.Select(ToEdge));
    public static int GetTime(this char c, int offset) => offset + c - 'A' + 1;
}