namespace AdventOfCode.Year2018.Day05;

partial class AoC
{
    static string input = File.ReadLines("input.txt").First();

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));

    public static int Part1(string input) => React(new StringBuilder(input), input, null);

    private static int React(StringBuilder sb, string input, char? ignore)
    {
        sb.Clear().Append(input);
        var length = sb.Length;
        sb = React(sb, ignore);
        while (sb.Length < length)
        {
            length = sb.Length;
            sb = React(sb, ignore);
        }
        var result = sb.Length;
        sb.Clear();
        return result;
    }

    private static StringBuilder React(StringBuilder input, char? ignore)
    {
        var i = 0;
        while (i < input.Length - 1)
        {
            // char arithmetic turns out to be much faster than ToLowerInvariant/ToUpperInvariant
            var diff = 'a' - 'A';
            var c1 = input[i];
            var c2 = input[i + 1];

            //if (c1 == ignore || char.ToLowerInvariant(c1) == ignore)
            if (c1 == ignore || (c1 + diff) == ignore)
            {
                input.Remove(i, 1);
                continue;
            }
            //if (c1 != c2 && char.ToUpperInvariant(c1) == char.ToUpperInvariant(c2))
            if (c1 != c2 && Math.Abs(c1 - c2) == diff)
            {
                input.Remove(i, 2);
            }
            else
            {
                i++;
            }
        }
        return input;
    }

    public static int Part2(string input)
    {
        var sb = new StringBuilder();
        var query = from c in Enumerable.Range('a', 26)
                    let length = React(sb, input, (char)c)
                    orderby length
                    select length;
        return query.First();
    }
}