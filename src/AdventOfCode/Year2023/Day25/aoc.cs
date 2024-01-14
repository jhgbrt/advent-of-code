
using Net.Code.Graph;
using Net.Code.Graph.Algorithms;
using AdventOfCode;

namespace AdventOfCode.Year2023.Day25;
public class AoC202325
{
    public AoC202325():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    IGraph<string, int> graph;
    public IGraph<string, int> Graph => graph;
    public AoC202325(string[] input, TextWriter writer)
    {
        var vertices = (from line in input
                 let s = line.Split(": ")
                 from n in s[1].Split(' ').Append(s[0])
                 select n
                 );

        var edges = (from line in input
                 let s = line.Split(": ")
                 let left = s[0]
                 from right in s[1].Split(' ')
                 from e in (Edge.Create(left, right, 1), Edge.Create(right, left, 1))
                 select e
                 );

        graph = GraphBuilder.Create<string, int>()
            .AddVertices(vertices)
            .AddEdges(edges)
            .BuildGraph();

        this.writer = writer;
    }

    public object Part1()
    {
        var (_, partition1, partition2, _) = StoerWagner.MinimumCut(graph);
        return partition1.Count * partition2.Count;
    }
    public object Part2() => -1;
}

public class AoC202325Tests
{
    private readonly AoC202325 sut;
    public AoC202325Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202325(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(15, sut.Graph.VertexCount);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(54, sut.Part1());
    }

    
}
