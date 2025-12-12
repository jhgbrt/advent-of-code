namespace AdventOfCode.Year2025.Day11;
using Graph = Dictionary<string, string[]>;
public class AoC202511(string[] input)
{
    public AoC202511() : this(Read.InputLines()) { }
    Graph graph = input.Select(l => l.Split(": ")).ToDictionary(p => p[0], p => p[1].Split(' '));
    public long Part1() => Count("you", graph, [], 0);
    public long Part2() => Count("svr", graph, [], 0b100);

    private long Count(
      string node,
      Graph graph,
      Dictionary<(string, int), long> cache, int state)
    {
        state = (state & 0b100, node) switch
        {
            (0, _) => 0,
            (_, "dac") => state | 0b01,
            (_, "fft") => state | 0b10,
            _ => state
        };

        var value = node switch
        {
            "out" => state == 0 || (state & 0b111) == 0b111 ? 1 : 0,
            _ when cache.TryGetValue((node, state), out var cached) => cached,
            _ => graph[node].Select(n => Count(n, graph, cache, state)).Sum()
        };
        cache[(node, state)] = value;
        return value;
    }
}

public class AoC202511Tests(ITestOutputHelper output)
{


    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202511(input);
        Assert.Equal(5, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines(2);
        foreach (var line in input) {
            output.WriteLine(line);
        }
        var sut = new AoC202511(input);
        Assert.Equal(2, sut.Part2());
    }
}