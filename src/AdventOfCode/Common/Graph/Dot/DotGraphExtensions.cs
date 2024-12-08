namespace Net.Code.Graph.Dot;

public static class DotGraphExtensions
{
    public static T WithRankDir<T>(this T graph, DotRankDir rankDir) where T : DotBaseGraph
    {
        graph.RankDir = new DotRankDirAttribute(rankDir);
        return graph;
    }

    public static T Add<T>(this T graph, DotElement element) where T : DotBaseGraph
    {
        graph.Elements.Add(element);
        return graph;
    }

    public static T AddNode<T>(this T graph, string id) where T : DotBaseGraph
    {
        return graph.Add(new DotNode().WithIdentifier(id));
    }
    public static T AddEdge<T>(this T graph, string from, string to, string label) where T : DotBaseGraph
    {
        return graph.Add(new DotEdge().From(from).To(to).WithLabel(label));
    }

    public static DotGraph Directed(this DotGraph graph, bool directed = true)
    {
        graph.Directed = directed;
        return graph;
    }

    public static DotGraph Strict(this DotGraph graph, bool strict = true)
    {
        graph.Strict = strict;
        return graph;
    }

    public static DotSubgraph WithColor(this DotSubgraph subgraph, string color)
    {
        subgraph.Color = new DotColorAttribute(color);
        return subgraph;
    }

    public static DotSubgraph WithColor(this DotSubgraph subgraph, DotColor color)
    {
        subgraph.Color = new DotColorAttribute(color);
        return subgraph;
    }

    public static DotSubgraph WithStyle(this DotSubgraph subgraph, string style)
    {
        subgraph.Style = new DotSubgraphStyleAttribute(style);
        return subgraph;
    }

    public static DotSubgraph WithStyle(this DotSubgraph subgraph, DotSubgraphStyle style)
    {
        subgraph.Style = new DotSubgraphStyleAttribute(style);
        return subgraph;
    }
    public static DotGraph ToDotGraph<TVertex, TValue>(this IGraph<TVertex, TValue> graph,
      Func<TVertex, DotNode> tonode,
      Func<Edge<TVertex, TValue>, DotEdge> toedge)
        where TVertex : IEquatable<TVertex>
    {
        var result = new DotGraph("G").Strict();
        foreach (var node in graph.Vertices) result.Add(tonode(node));
        foreach (var edge in graph.Edges) result.Add(toedge(edge));
        return result;
    }
    public static DotGraph ToDotGraph<TVertex, TValue>(this IGraph<TVertex, TValue> graph, IReadOnlyDictionary<TVertex, string> labels)
        where TVertex : IEquatable<TVertex>
    {
        var result = new DotGraph("G").Strict();
        foreach (var node in graph.Vertices)
        {
            var dotnode = new DotNode().WithIdentifier(node.ToString()!);
            if (labels.TryGetValue(node, out var label))
                dotnode = dotnode.WithLabel($"{label}({node})");
            result.Add(dotnode);
        }
        foreach (var edge in graph.Edges) result.Add(new DotEdge().From(edge.Source.ToString()!).To(edge.Destination.ToString()!).WithLabel(edge.Value!.ToString()!));
        result.Directed(false /* TODO */);
        return result;
    }
}