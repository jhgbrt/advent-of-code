using System.Collections.Concurrent;

namespace AdventOfCode.Year2023.Day12;
public class AoC202312
{
    public AoC202312() : this(Read.InputLines(), Console.Out) { }

    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    TextWriter writer;

    public AoC202312(string[] input, TextWriter writer)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public long Part1() => (
        from item in items
        select Count(item.Append())
        ).Sum();

    public long Part2() => (
        from item in items
        select Count(item.Expand())
        ).Sum();

    private long Count(Item item) => Count([], item, new(0, 0, 0));

    private long Count(ConcurrentDictionary<Key, long> cache, Item item, Key key)
    {
        if (cache.TryGetValue(key, out long c)) return c;

        var count = (item.GetCharOrDefault(key.i), key.cnt - item.numbers.Length) switch
        {
            (null, _) => item.numbers.Length == key.cnt ? 1 : 0,
            ('#', _) => Count(cache, item, new(key.i + 1, key.cur + 1, key.cnt)),
            ('.', _) or (_, 0) => Recurse(cache, item, key),
            _ => Count(cache, item, new(key.i + 1, key.cur + 1, key.cnt)) + Recurse(cache, item, key)
        };

        cache[key] = count;
        return count;
    }

    private long Recurse(ConcurrentDictionary<Key, long> cache, Item item, Key key) => key.cur switch
    {
        0 => Count(cache, item, new(key.i + 1, 0, key.cnt)),
        _ when item.GetNumberAt(key.cnt, out var v) && v == key.cur
            => Count(cache, item, new(key.i + 1, 0, key.cnt + 1)),
        _ => 0
    };

}
readonly record struct Key(int i, int cur, int cnt);
readonly record struct Item(string layout, int[] numbers)
{
    public Item Append() => this with { layout = $"{layout}."};
    public Item Expand() => this with
    {
        layout = $"{string.Join('?', Repeat(layout, 5))}.",
        numbers = Repeat(numbers, 5).SelectMany(n => n).ToArray()
    };
    public bool GetNumberAt(int index, out int value)
    {
        if (index < numbers.Length)
        {
            value = numbers[index];
            return true;
        }
        value = -1;
        return false;
    }

    public char? GetCharOrDefault(int index)
    {
        if (index < layout.Length) return layout[index];
        return null;
    }
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<layout>[\?#\.]*) (?<numbers>[\d,]+)$")]
    public static partial Regex MyRegex();
}
public class AoC202312Tests
{
    private readonly AoC202312 sut;
    private readonly TextWriter output;

    public AoC202312Tests(ITestOutputHelper testoutput)
    {
        var input = Read.SampleLines();
        output = new TestWriter(testoutput);
        sut = new AoC202312(input, output);
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(6, sut.Items.Count());
        Assert.Equal("???.###", sut.Items.First().layout);
        var expected = new[] { 1, 1, 3 };
        var actual = sut.Items.First().numbers;
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(21, sut.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        Assert.Equal(525152, sut.Part2());
    }

    [Fact]
    public void TestExpand()
    {
        var item = new Item("#.?#", [1, 2]).Expand();
        Assert.Equal("#.?#?#.?#?#.?#?#.?#?#.?#.", item.layout);
        Assert.Equal(new[] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 2 }, item.numbers);
    }
}