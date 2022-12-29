namespace AdventOfCode.Year2016.Day20;

public partial class AoC201620
{
    static Regex regex = MyRegex();
    static string[] input = Read.InputLines();
    static ImmutableArray<Range> ranges = (from line in input
                                           let range = regex.As<Range>(line)
                                           orderby range.start, range.end
                                           select range
                                           ).Merge().ToImmutableArray();


    public object Part1()
    {
        var q = from i in Range(0, int.MaxValue)
                where !ranges.Any(r => r.Contains((uint)i))
                select i;

        return q.First();
    }
    public object Part2()
    {
        // TODO this is not correct yet
        return (long)(uint.MaxValue) - ranges.Sum(r => r.Total);
    }

    [GeneratedRegex("(?<start>\\d+)-(?<end>\\d+)", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}

readonly record struct Range(long start, long end)
{
    public bool Contains(long i) => i >= start && i <= end;
    public long Total => end - start + 1;
}

static class Ex
{
    internal static IEnumerable<Range> Merge(this IEnumerable<Range> ranges)
    {
        var enumerator = ranges.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var (start, end) = enumerator.Current;
        while (enumerator.MoveNext())
        {
            if (enumerator.Current.start > end + 1)
            {
                yield return new(start, end);
                (start, end) = enumerator.Current;
            }
            else
            {
                end = Max(end, enumerator.Current.end);
            }
        }
        yield return new(start, end);
    }
}