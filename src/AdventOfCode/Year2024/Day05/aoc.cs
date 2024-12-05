namespace AdventOfCode.Year2024.Day05;

public class AoC202405(Stream stream)
{
    public AoC202405() : this(Read.InputStream()) {}

    (IList<(int left, int right)> rules, IList<IList<int>> updates) input = ReadInput(stream);
    IComparer<int> comparer => new CustomComparer(input.rules);

    private static (IList<(int, int)>, IList<IList<int>>) ReadInput(Stream stream)
    {
        IList<(int, int)> rules = [];
        IList<IList<int>> updates = [];

        var sr = new StreamReader(stream);

        var readingRules = true;

        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (readingRules && line == "")
            {
                readingRules = false;
                continue;
            }
            if (string.IsNullOrEmpty(line)) continue;
            if (readingRules)
            {
                var separator = line.IndexOf('|');
                rules.Add((int.Parse(line[..separator]), int.Parse(line[(separator+1)..])));
            }
            else
            {
                var splits = line.Split( ',').Select(int.Parse).ToList();
                updates.Add(splits);
            }

        }

        return (rules, updates);
    }


    public int Part1() => (
            from update in input.updates
            where !InvalidRules(update).Any()
            select update[update.Count / 2]
            ).Sum();


    public int Part2() => (
            from update in (
                from update in input.updates
                where InvalidRules(update).Any()
                select update.Order(comparer).ToList()
                ).ToList()
            select update[update.Count / 2]
            ).Sum();
    IEnumerable<(int left, int right)> InvalidRules(IList<int> update)
        => from rule in input.rules
           where update.Contains(rule.left) && update.Contains(rule.right)
               && update.IndexOf(rule.left) > update.IndexOf(rule.right)
           select rule;
}

struct CustomComparer(IList<(int left, int right)> rules) : IComparer<int>
{
    public int Compare(int x, int y)
    {
        foreach (var (left, right) in rules)
        {
            if (left == x && right == y) return -1;
            if (left == y && right == x) return 1;
        }
        return x.CompareTo(y);
    }
}


public class AoC202405Tests
{
    private readonly AoC202405 sut;
    public AoC202405Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202405(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(143, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(123, sut.Part2());
    }
}