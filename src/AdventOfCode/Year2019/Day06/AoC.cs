using QuickGraph;
using QuickGraph.Algorithms;
namespace AdventOfCode.Year2019.Day06;

public class AoC201906
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
    public static int Part1(string[] input)
    {
        var graph = input.CreateGraph();
        return graph.Vertices
            .Select(v => graph.CountDistance("COM", v))
            .Sum();
    }

    public static int Part2(string[] input)
        => input.CreateGraph().CountDistance("YOU", "SAN") - 2;
}


static class Ex
{
    public static IUndirectedGraph<string, SEdge<string>> CreateGraph(this string[] input)
        => input
            .Select(s => s.Split(')'))
            .Select(s => new SEdge<string>(s[0], s[1]))
            .ToUndirectedGraph<string, SEdge<string>>();

    public static int CountDistance(this IUndirectedGraph<string, SEdge<string>> graph, string from, string to)
        => !graph.ShortestPathsDijkstra(e => 1, from)(to, out var edges) ? 0 : edges.Count();


}