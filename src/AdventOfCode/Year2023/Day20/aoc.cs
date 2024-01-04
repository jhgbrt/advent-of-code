namespace AdventOfCode.Year2023.Day20;
public class AoC202320
{
    public AoC202320():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly IEnumerable<IModule> modules;
    internal IEnumerable<IModule> Items => modules;
    public AoC202320(string[] input, TextWriter writer)
    {
        var info = (
            from line in input
            let split = line.Split(" -> ")
            let s = split[0]
            let p = s[0] switch
            {
                '&' or '%' => (type: s[0], name: s[1..]),
                _ => (type: default, name: s)
            }
            let targets = split[1].Split(", ")
            select (p.type, source: p.name, targets)
        ).ToList();

        var sourcesByTarget = (
            from item in info
            from target in item.targets
            group item.source by target
            ).ToDictionary(x => x.Key, g => g);

        var modulenames = sourcesByTarget.Values.SelectMany(p => p).ToHashSet();

        var parsed = (from item in info
                      let sources = sourcesByTarget.ContainsKey(item.source) ? sourcesByTarget[item.source] : Enumerable.Empty<string>()
                      let targets = item.targets
                      select item.type switch
                      {
                          '&' => new Conjunction(item.source, sources, item.targets),
                          '%' => new FlipFlop(item.source, sources, item.targets, false),
                          _ => new Module(item.source, sources, item.targets) as IModule
                      });

        var missing = from p in parsed 
                      from t in p.Targets
                      where !modulenames.Contains(t)
                      select new Module(t, [p.Name], []);

        // do not materialize to automatically reset state between parts
        modules = parsed.Concat(missing);

        this.writer = writer;
    }

    public long Part1()
    {
        var modules = this.modules.ToImmutableDictionary(m => m.Name);
   
        long low = 0, high = 0;

        var queue = new Queue<Pulse>();
        for (int i = 0; i < 1000; i++)
        {
            queue.Enqueue(new ("button", "broadcaster", false));
            
            while (queue.TryDequeue(out var p))
            {
                if (p.high) high++;
                else low++;            
                var outputs = modules[p.target].Process(p);
                foreach (var output in outputs)
                {
                    queue.Enqueue(output);
                }
            }
        }

        return low * high;
    }
    public long Part2() => Part2("rx");
    internal long Part2(string target) 
    {
        var modules = this.modules.ToImmutableDictionary(m => m.Name);

        var level1 = modules[target].Sources.Single();
        var level2 = modules[level1].Sources;
        var seen = new Dictionary<string, long>();
        var n = 0L;

        var queue = new Queue<Pulse>();

        while (true)
        {
            n++;
            queue.Enqueue(new("button", "broadcaster", false));
            while (queue.TryDequeue(out var p))
            {
                if (p.target == level1 && p.high)
                {
                    seen.TryAdd(p.source, n);
                    if (level2.All(seen.ContainsKey))
                    {
                        return seen.Values.Aggregate(1L, (a, v) => a * v);
                    }
                }

                var outputs = modules[p.target].Process(p);

                foreach (var output in outputs)
                {
                    queue.Enqueue(output);
                }
            }
        }
    }


}

interface IModule 
{ 
    string Name { get; }
    IEnumerable<string> Targets { get; }
    IEnumerable<string> Sources { get; }
    IEnumerable<Pulse> Process(Pulse pulse);
}
class Conjunction(string name, IEnumerable<string> sources, IEnumerable<string> targets) : IModule
{
    HashSet<string> memory = [];
    public string Name => name;
    public IEnumerable<string> Targets => targets;
    public IEnumerable<string> Sources => sources;
    public IEnumerable<Pulse> Process(Pulse pulse)
    {
        if (pulse.high) 
            memory.Add(pulse.source);
        else if (memory.Contains(pulse.source)) 
            memory.Remove(pulse.source);

        var value = !sources.All(x => memory.Contains(x));
        return targets.Select(t => new Pulse(name, t, value));
    }
    public override string ToString() => $"&{name}";
}
class FlipFlop(string name, IEnumerable<string> sources, IEnumerable<string> targets, bool on) : IModule
{
    public string Name => name;
    public IEnumerable<string> Targets => targets;
    public IEnumerable<string> Sources => sources;
    public IEnumerable<Pulse> Process(Pulse pulse)
    {
        if (!pulse.high)
        {
            on = !on;
            return targets.Select(t => new Pulse(name, t, on));
        }
        else
        {
            return [];
        }
    }
    public override string ToString() => $"%{name}";
}
class Module(string name, IEnumerable<string> sources, IEnumerable<string> targets) : IModule
{
    public string Name => name;
    public IEnumerable<string> Targets => targets;
    public IEnumerable<string> Sources => sources;
    public IEnumerable<Pulse> Process(Pulse pulse)
    {
        return targets.Select(t => new Pulse(name, t, pulse.high));
    }
    public override string ToString() => $"{name}";
}

record struct Pulse(string source, string target, bool high)
{
    public override string ToString() => $"{source} -{(high ? "high" : "low")}-> {target}";
}

public class AoC202320Tests
{
    private readonly ITestOutputHelper output;
    public AoC202320Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestParsing1()
    {
        var input = Read.Sample(1).Lines().ToArray();
        var sut = new AoC202320(input, new TestWriter(output));
        Assert.Equal(5, sut.Items.Count());
        Assert.Equal(new[] { "broadcaster", "a", "b", "c", "inv" }, sut.Items.Select(m => m.Name));
    }
    [Fact]
    public void TestParsing2()
    {
        var input = Read.Sample(2).Lines().ToArray();
        var sut = new AoC202320(input, new TestWriter(output));
        Assert.Equal(6, sut.Items.Count());
        Assert.Equal(new[] { "broadcaster", "a", "inv", "b", "con", "output"}, sut.Items.Select(m => m.Name));
    }

    [Theory]
    [InlineData(1, 32000000L)]
    [InlineData(2, 11687500L)]
    public void TestPart1(int sample, long expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        var sut = new AoC202320(input, new TestWriter(output));
        Assert.Equal(expected, sut.Part1());
    }
}
