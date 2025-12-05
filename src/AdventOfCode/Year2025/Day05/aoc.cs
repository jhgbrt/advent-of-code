namespace AdventOfCode.Year2025.Day05;

record struct Range(long start, long end) : IComparable<Range>
{
    public static Range Parse(ReadOnlySpan<char> s) 
    {
        var split = s.IndexOf('-');
        var start = long.Parse(s[..split]);
        var end = long.Parse(s[++split..]);
        return new Range(start, end);
    }
    public bool Contains(long value) => start <= value && end >= value;
    public bool Overlaps(Range other) => start <= other.end && end >= other.start;
    public Range Merge(Range other) => new Range(Math.Min(start, other.start), Math.Max(end, other.end));

    public int CompareTo(Range other)
    {
        int cmp = start.CompareTo(other.start);
        if (cmp != 0) return cmp;
        return end.CompareTo(other.end);
    }
    public long Count = end - start + 1;
}

public class AoC202505
{
    Range[] ranges;
    long[] ids;
    public AoC202505(ReadOnlySpan<string> input)
    {
        var split = input.IndexOf("");
        ranges = new Range[split];
        for (int i = 0; i < split; i++) 
        {
            ranges[i] = Range.Parse(input[i]);
        }
        ranges.Sort();
        var nofids = input.Length - split - 1;
        ids = new long[nofids];
        for (int i = 0; i < nofids; i++)
        {
            ids[i] = long.Parse(input[i + split + 1]);
        }
        ids.Sort();
    }

    public AoC202505() : this(Read.InputLines()) { }

    public int Part1()
    {
        var count = 0;
        var j = 0;
        for (int i = 0; i< ids.Length; i++)
        {
            // advance j to the first range that could contain ids[i]
            while (j < ranges.Length && ranges[j].end < ids[i])
            {
                j++;
            }
            if (j < ranges.Length && ranges[j].Contains(ids[i]))
            {
                count++;
            }
        }
        return count;
    }
    public long Part2()
    {
        var merged = ranges[0];
        long count = 0;

        for (int i = 1; i < ranges.Length; i++)
        {
            if (merged.Overlaps(ranges[i]))
            {
                merged = merged.Merge(ranges[i]);
            }
            else
            {
                count += merged.Count;
                merged = ranges[i];
            }
        }

        // last one is either merged or standalone; has to be added in any case
        count += merged.Count;

        return count;

    }
}

public class AoC202505Tests
{
    private readonly AoC202505 sut;
    public AoC202505Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202505(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(3, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(14, sut.Part2());
    }


}