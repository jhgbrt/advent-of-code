namespace AdventOfCode.Year2024.Day05;

public class AoC202405(string[] lines)
{
    public AoC202405() : this(Read.InputLines()) {}

    ((int left, int right)[] rules, int[][] updates) input = ReadInput(lines);

    private static ((int left, int right)[], int[][]) ReadInput(string[] lines)
    {
        var rules = from line in lines.TakeWhile(l => l != "")
                    let parts = line.Split('|')
                    select (int.Parse(parts[0]), int.Parse(parts[1]));

        var updates = from line in lines.SkipWhile(l => l != "").Skip(1)
                      select line.Split(',').Select(int.Parse).ToArray();

        return (rules.ToArray(), updates.ToArray());
    }


    public int Part1() => (
            from update in input.updates
            where !InvalidRules(update).Any()
            select update[update.Length / 2]
            ).Sum();

    public int Part2()
    {
        var comparer = new CustomComparer(input.rules);
        return (
            from update in input.updates
            where InvalidRules(update).Any()
            select update.Order(comparer).Skip(update.Length / 2).First()
            ).Sum();
    }

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
        var input = Read.SampleLines();
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