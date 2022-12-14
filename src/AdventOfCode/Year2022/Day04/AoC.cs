namespace AdventOfCode.Year2022.Day04;
public class AoC202204
{
    static string[] input = Read.InputLines();
    public int Part1() => (from line in input
                           let pair = RangePair.Parse(line)
                           where pair.left.Contains(pair.right) || pair.right.Contains(pair.left)
                           select pair).Count();
    public int Part2() => (from line in input
                           let pair = RangePair.Parse(line)
                           where pair.left.Overlaps(pair.right)
                           select pair).Count();
}

partial record struct Range(int start, int end)
{
    public bool Contains(Range other) => start <= other.start && end >= other.end;
    public bool Overlaps(Range other) => start <= other.end && end >= other.start;

    public static Range Parse(string s) => _r.As<Range>(s)!.Value;
    static Regex _r = MyRegex();

    [GeneratedRegex("^(?<start>\\d+)-(?<end>\\d+)$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}

record struct RangePair(Range left, Range right)
{
    static Regex _r = new(@"^(?<left>[^,]+),(?<right>[^,]+)$");
    public static RangePair Parse(string s)
    {
        var match = _r.Match(s);
        return new(
            Range.Parse(match.Groups["left"].Value),
            Range.Parse(match.Groups["right"].Value)
            );
    }
}

public class Tests
{
    [Theory]
    [InlineData(1, 5, 1, 3, true)]
    [InlineData(1, 1, 1, 1, true)]
    [InlineData(1, 5, 6, 7, false)]
    [InlineData(1, 5, 3, 7, false)]
    public void RangeContains(int l1, int u1, int l2, int u2, bool expected)
    {
        Assert.Equal(expected, new Range(l1, u1).Contains(new Range(l2, u2)));
    }

    [Theory]
    [InlineData(1, 5, 1, 3, true)]
    [InlineData(1, 1, 1, 1, true)]
    [InlineData(1, 5, 6, 7, false)]
    [InlineData(1, 5, 3, 7, true)]
    public void RangeOverlaps(int l1, int u1, int l2, int u2, bool expected)
    {
        Assert.Equal(expected, new Range(l1, u1).Overlaps(new Range(l2, u2)));
    }
}