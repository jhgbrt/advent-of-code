namespace AdventOfCode.Year2021.Day10;

public class AoC202110
{
    static string[] input = Read.InputLines(typeof(AoC202110));
    
    public object Part1() => (
        from line in input
        select Score1(line)
    ).Sum();

    static int Score1(string line)
    {
        var open = ImmutableStack<char>.Empty;

        foreach (var c in line)
        {
            (var score, open) = c switch
            {
                '{' or '[' or '(' or '<' => (0, open.Push(c)),
                ')' when open.Peek() != '(' => (3, open),
                ']' when open.Peek() != '[' => (57, open),
                '}' when open.Peek() != '{' => (1197, open),
                '>' when open.Peek() != '<' => (25137, open),
                _ => (0, open.Pop())
            };
            if (score != 0) return score;
        }

        return 0;
    }

    public object Part2() => (from line in input
                                       where Score1(line) == 0
                                       let score = Score2(line)
                                       orderby score
                                       select score
                                       ).Median();

    long Score2(string line)
    {
        var open = line.Aggregate(
            ImmutableStack<char>.Empty,
            (stack, c) => c switch { '{' or '[' or '(' or '<' => stack.Push(c), _ => stack.Pop() }
            );

        return (from c in open
                select c switch
                {
                    '<' => 4, '{' => 3, '[' => 2, '(' => 1,
                    _ => throw new Exception()
                }).Aggregate(0L, (total, i) => 5 * total + i);
    }
}


static class Extension
{
    public static long Median(this IEnumerable<long> input) => input.OrderBy(n => n).ElementAt(input.Count() / 2);
}