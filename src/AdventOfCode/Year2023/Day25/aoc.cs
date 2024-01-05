using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.ConnectedComponents;

namespace AdventOfCode.Year2023.Day25;
public class AoC202325
{
    public AoC202325():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableList<string> nodes;
    readonly ImmutableList<(string left, string right)> edges;
    public IEnumerable<string> Nodes => nodes;
    public IEnumerable<(string left, string right)> Edges => edges;
    public AoC202325(string[] input, TextWriter writer)
    {
        nodes = (from line in input
                 let s = line.Split(": ")
                 from n in s[1].Split(' ').Append(s[0])
                 select n
                 ).Distinct().ToImmutableList();

        edges = (from line in input
                 let s = line.Split(": ")
                 let left = s[0]
                 from right in s[1].Split(' ')
                 select (left, right).AsEnumerable().Order().ToTuple2()
                 ).Distinct().ToImmutableList(); 


        this.writer = writer;
    }

    public object Part1()
    {
        //foreach (var e in edges.GetPermutations(3))
        {
            //var graph = edges.Except(e).Select(p => p.ToEdge()).ToUndirectedGraph();

            //Dictionary<string, int> components = [];
            //var count = graph.ConnectedComponents(components);

            //var algo = new ConnectedComponentsAlgorithm<string, SEdge<string>>(graph);
            //algo.Compute();
            //var count = algo.ComponentCount;
            //writer.WriteLine(count);

            //if (count == 3)
            //{
            //    Debugger.Break();
            //}
        }


        return -1;
    }
    public object Part2() => "";
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
        Assert.Equal(15, sut.Nodes.Count());
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(3, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}

public static class GraphExtensions
{
    public static UndirectedGraph<T, SEdge<T>> ToUndirectedGraph<T>(this IEnumerable<SEdge<T>> edges) => edges.ToUndirectedGraph<T, SEdge<T>>();
    public static SEdge<T> ToEdge<T>(this (T, T) p) => new SEdge<T>(p.Item1, p.Item2);
}