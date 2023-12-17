namespace AdventOfCode;

static class StringExtensions
{
    internal static int ToInt32(this string s, char one)
    {
        var bits = new string(s.Select(c => c == one ? '1' : '0').ToArray());
        return Convert.ToInt32(bits, 2);
    }
    internal static IEnumerable<Range> Lines(this string s)
    {
        int x = 0;
        while (x < s.Length)
        {
            var newline = s.IndexOf('\n', x);
            if (newline == -1)
            {
                if (!s.EndsWith('\n'))
                {
                    yield return new(x, s.Length);
                }
                break;
            }
            var count = newline switch { > 0 when s[newline - 1] == '\r' => newline - x - 1, _ => newline - x };
            yield return new(x, x + count);
            x = newline + 1;
        }
    }
    internal static IEnumerable<Range> Chunk(this Range range, int size)
    {
        int s = range.Start.Value;
        while (s < range.End.Value)
        {
            yield return new Range(s, s + (size > range.End.Value ? range.End.Value - s : size));
            s += size;
        }
    }
}