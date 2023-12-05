namespace AdventOfCode.Year2023.Day05;
public class AoC202305
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] realinput = Read.InputLines();
    static string[] input = usesample ? sample : realinput;
    static long[] seeds;
    static Map[] maps;
    static AoC202305()
    {
        List<Map> maps = new();
        List<Range> ranges = new();

        seeds = input[0].Split(": ")[1].Split(' ').Select(long.Parse).ToArray();
        string source = string.Empty;
        string destination = string.Empty;

        for (int i = 1; i < input.Length; i++)
        {
            string line = input[i];
            if (line == "")
            {
                if (ranges.Any())
                    maps.Add(new Map(source, destination, ranges.ToArray()));
                ranges.Clear();
                source = string.Empty;
                destination = string.Empty;

            }
            else if (line.EndsWith("map:"))
            {
                var match = Regexes.CardRegex().Match(line);
                source = match.Groups["source"].Value;
                destination = match.Groups["destination"].Value;
            }
            else
            {
                var numbers = line.Split(' ').Select(long.Parse).ToArray();
                var (destinationStart, sourceStart, length) = (numbers[1], numbers[0], numbers[2]);
                ranges.Add(new(sourceStart, destinationStart, length));
            }
        }
        AoC202305.maps = maps.ToArray();
    }

    public object Part1() => FindLocations().Min();

    private IEnumerable<long> FindLocations()
       {
        foreach (var seed in seeds)
        {
            long value = seed;
            foreach (var map in maps)
            {
                value = map.Find(value);
            }
            yield return value;
        }

    }

    public object Part2() => "";
}

readonly record struct Range(long source, long destination, long length)
{
    private long offset => source - destination;
    public bool Contains(long value) => value >= source && value < source + length;
    public long Map(long value) => value >= source && value < source + length ? value - offset : value;
}

class Map(string source, string destination, Range[] ranges)
{
    public long Find(long value)
    {
        foreach (var range in ranges)
        {
            if (range.Contains(value))
                return range.Map(value);
        }
        return value;
    }
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<source>[^-]*)-to-(?<destination>[^ ]*) map:$")]
    public static partial Regex CardRegex();
}

public class Tests
{
    [Fact]
    public void Range_Contains()
    {
        var range = new Range(98, 50, 2);
        Assert.True(range.Contains(98));
        Assert.True(range.Contains(99));
        Assert.False(range.Contains(100));
        Assert.Equal(50, range.Map(98));

    }
    [Fact]
    public void Range2()
    {
        var range = new Range(50, 52, 48);
        var mapped = range.Map(79);
        Assert.Equal(81, mapped);
    }

}