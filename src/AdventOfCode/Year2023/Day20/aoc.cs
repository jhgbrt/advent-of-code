using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace AdventOfCode.Year2023.Day20;
public class AoC202320
{
    public AoC202320():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<IModule> modules;
    readonly ImmutableDictionary<string, ImmutableArray<string>> links;
    internal IEnumerable<IModule> Items => modules;
    public AoC202320(string[] input, TextWriter writer)
    {
        var parsed = (from line in input
                  let split = line.Split(" -> ")
                  let name = split[0]
                  let module = name[0] switch
                  {
                      '&' => new Conjunction(name[1..]),
                      '%' => new FlipFlop(name[1..], false),
                      _ => new Module(name) as IModule
                  }
                  let targets = split[1].Split(", ").ToImmutableArray()
                  select (module, targets)).ToImmutableArray();

        modules = parsed.Select(p => p.module).ToImmutableArray();
        links = parsed.ToImmutableDictionary(p => p.module.name, p => p.targets);
        this.writer = writer;
    }

    public object Part1()
    {
        var modules = this.modules.ToImmutableDictionary(m => m.name);
        var broadcaster = modules["broadcaster"];

        var targets = links[broadcaster.name].Select(n => modules[n]);
        

        foreach (var target in targets) 
        {
            
        }

        foreach (var item in modules)
        {
            
            writer.WriteLine(item);
        }

        return -1;
    }
    public object Part2() => "";


}

interface IModule 
{ 
    string name { get; }
    void High();
    void Low();
}
readonly record struct Conjunction(string name) : IModule
{
    public void High()
    {
    }

    public void Low()
    {
    }

    public override string ToString() => $"&{name}";
}
readonly record struct FlipFlop(string name, bool on) : IModule
{
    public void High()
    {
    }

    public void Low()
    {
    }

    public override string ToString() => $"%{name}";
}
readonly record struct Module(string name) : IModule
{
    public void High()
    {
    }

    public void Low()
    {
    }
}

public class AoC202320Tests
{
    private readonly AoC202320 sut;
    private readonly ITestOutputHelper output;
    public AoC202320Tests(ITestOutputHelper output)
    {
        var input = Read.Sample(1).Lines().ToArray();
        sut = new AoC202320(input, new TestWriter(output));
        this.output = output;
    }

    [Fact]
    public void TestParsing1()
    {
        var input = Read.Sample(1).Lines().ToArray();
        var sut = new AoC202320(input, new TestWriter(output));
        Assert.Equal(5, sut.Items.Count());
        Assert.Equal(new[] { "broadcaster", "a", "b", "c", "inv" }, sut.Items.Select(m => m.name));
    }
    [Fact]
    public void TestParsing2()
    {
        var input = Read.Sample(2).Lines().ToArray();
        var sut = new AoC202320(input, new TestWriter(output));
        Assert.Equal(5, sut.Items.Count());
        Assert.Equal(new[] { "broadcaster", "a", "inv", "b", "con" }, sut.Items.Select(m => m.name));
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(-1, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}
