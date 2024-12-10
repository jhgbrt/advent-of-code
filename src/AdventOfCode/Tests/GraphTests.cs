using Net.Code.Graph;

namespace AdventOfCode.Tests;

public class GraphTraversalTests
{
    /*
     * 0 -> 1 -> 4
     * |    |   
     * |    + -> 5
     * |   
     * | -> 2 -> 6
     * |   
     * + -> 3 -> 7
     * 
     */

    IGraph<string, int> graph = GraphBuilder.Create<string, int>()
        .AddEdges(
            [
                Edge.Create("0", "1", 1),
                Edge.Create("0", "2", 1),
                Edge.Create("0", "3", 1),
                Edge.Create("1", "4", 1),
                Edge.Create("1", "5", 1),
                Edge.Create("2", "6", 1),
                Edge.Create("3", "7", 1)
            ]
        ).BuildGraph();


    [Fact]
    public void TestDepthFirst()
    {
        var result = graph.DepthFirst("0").ToList();
        Assert.Equal(new[] { "0", "3", "7", "2", "6", "1", "5", "4" }, result);
    }

    [Fact]
    public void TestBreadthFirst()
    {
        var result = graph.BreadthFirst("0").ToList();
        Assert.Equal(new[] { "0", "1", "2", "3", "4", "5", "6", "7" }, result);
    }

}