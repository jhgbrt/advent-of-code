namespace AdventOfCode.Year2022.Day04;
public class AoC202204
{
    static string[] input = Read.InputLines();
    public int Part1() => (from line in input
                           let pair = RangeEx.ParsePair(line)
                           where pair.left.Contains(pair.right) || pair.right.Contains(pair.left)
                           select pair).Count();
    public int Part2() => (from line in input
                           let pair = RangeEx.ParsePair(line)
                           where pair.left.Overlaps(pair.right)
                           select pair).Count();
}

static partial class RangeEx
{
    public static bool Contains(this Range left, Range right)
        => left.Start.Value <= right.Start.Value && left.End.Value >= right.End.Value;
    public static bool Overlaps(this Range left, Range right)
        => left.Start.Value <= right.End.Value && left.End.Value >= right.Start.Value;

    static Regex _regex = Regex();
    [GeneratedRegex(@"^(?<l1>\d+)-(?<u1>\d+),(?<l2>\d+)-(?<u2>\d+)$")]
    private static partial Regex Regex();
    public static (Range left, Range right) ParsePair(string s)
    {
        var match = _regex.Match(s);
        return (
            new(int.Parse(match.Groups["l1"].Value), int.Parse(match.Groups["u1"].Value)),
            new(int.Parse(match.Groups["l2"].Value), int.Parse(match.Groups["u2"].Value))
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