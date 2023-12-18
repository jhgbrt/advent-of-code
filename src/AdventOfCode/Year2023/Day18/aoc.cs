
namespace AdventOfCode.Year2023.Day18;



public class AoC202318
{
    public AoC202318():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    string[] input;
    readonly ImmutableArray<Item> items1;
    readonly ImmutableArray<Item> items2;
    internal IEnumerable<Item> Items1 => items1;
    internal IEnumerable<Item> Items2 => items2;
    public AoC202318(string[] input, TextWriter writer)
    {
        this.input = input;
        items1 = input.Select(s => Regexes.RegexPart1().As<Item>(s)).ToImmutableArray();
        items2 = input.Select(s =>
            {
                var match = Regexes.RegexPart2().Match(s);
                var distance = Convert.ToInt32(match.Groups["distance"].Value, 16);
                var dir = match.Groups["dir"].Value[0] switch { '0' => 'R', '1' => 'D', '2' => 'L', '3' => 'U' };
                return new Item(dir, distance);
            }
        ) .ToImmutableArray();
        this.writer = writer;
    }

    public long Part1() => Solve(items1);

    public long Part2() => Solve(items2);

    private long Solve(IEnumerable<Item> items)
    {
        var q = from item in items
                select (item.distance,
                    dx: item.direction switch { 'R' => item.distance, 'L' => -item.distance, _ => 0 },
                    dy: item.direction switch { 'D' => item.distance, 'U' => -item.distance, _ => 0 }
                );

        // https://en.wikipedia.org/wiki/Pick%27s_theorem
        // https://en.wikipedia.org/wiki/Shoelace_formula
        var current = (x: 0L, y: 0L);
        var (area, perimeter) = (0L, 0L);
        foreach (var (distance, dx, dy) in q)
        {
            var next = (x: current.x + dx, y: current.y + dy);
            area += (current.x * next.y) - (current.y * next.x);
            perimeter += distance;
            current = next;
        }
        area = (area + current.x - current.y)/2;
        area = area + (perimeter / 2) + 1;
        return area;
    }

}

readonly record struct Item(char direction, int distance);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<direction>[RLUD]) (?<distance>\d+) .*$")]
    public static partial Regex RegexPart1();
    [GeneratedRegex(@"^[RLUD] \d+ \(#(?<distance>\w{5})(?<dir>\w)\)$")]
    public static partial Regex RegexPart2();
}

public class AoC202318Tests
{
    private readonly AoC202318 sut;
    public AoC202318Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202318(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(14, sut.Items1.Count());
        Assert.Equal(6, sut.Items1.First().distance);
        Assert.Equal('R', sut.Items1.First().direction);
        Assert.Equal(461937, sut.Items2.First().distance);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(62, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(952408144115, sut.Part2());
    }

}

