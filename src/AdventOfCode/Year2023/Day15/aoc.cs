using QuickGraph;

namespace AdventOfCode.Year2023.Day15;
public class AoC202315
{
    public AoC202315():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    string[] items;
    public AoC202315(string[] input, TextWriter writer)
    {
        this.items = input[0].Split(',');
        this.writer = writer;
    }

    public object Part1() => items.Select(Hash).Sum();
    public object Part2() 
    {
        var instructions = items.Select(s => Regexes.MyRegex().As<Item>(s)).ToArray();
        var boxes = instructions.Select(x => x.Box).Distinct().ToDictionary(i => i, _ => new List<Item>());

        var query = from item in instructions
                    let box = boxes[item.Box]
                    let index = box.Select((it, idx) => (it.label, idx)).Where(p => p.label == item.label).Select(p => p.idx as int?).FirstOrDefault()
                    select (box, item, index);

        foreach (var (box, item, index) in query)
        {
            switch (item.operation, index)
            {
                case ('=', not null): box[index.Value] = item; break;
                case ('=', null): box.Add(item); break;
                case ('-', not null): box.RemoveAt(index.Value); break;
            }
        }

        var result = from b in boxes
                     from x in b.Value.Select((it, i) => (item: it, slot: i + 1))
                     let lens = x.item
                     select (lens.Box + 1) * x.slot * x.item.value!.Value;

        return result.Sum();
    }

    public int Hash(string input) => input.Aggregate(0, (c, i) => (i+c) * 17 % 256);

}

public record struct Item(string label, char operation, int? value) 
{
    public int Box => label.Aggregate(0, (c, i) => (i + c) * 17 % 256);
}
static partial class Regexes
{
    [GeneratedRegex(@"^(?<label>[a-z]+)(?<operation>(=|-))(?<value>\d*)$")]
    public static partial Regex MyRegex();
}

public class AoC202315Tests
{
    private readonly AoC202315 sut;
    public AoC202315Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202315(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(1320, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(145, sut.Part2());
    }

    [Fact]
    public void TestHash()
    {
        Assert.Equal(52, sut.Hash("HASH"));
    }
}