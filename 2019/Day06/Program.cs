using QuickGraph;
using QuickGraph.Algorithms;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));
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