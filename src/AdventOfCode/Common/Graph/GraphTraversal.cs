namespace Net.Code.Graph;

public static class GraphTraversal
{
    public static IEnumerable<(TVertex vertex, Edge<TVertex, TValue> edge)> GetEdges<TVertex, TValue>(
        this IGraph<TVertex, TValue> graph,
        IEnumerable<TVertex> vertices
        ) where TVertex : IEquatable<TVertex>, IComparable<TVertex>
    {
        foreach (var v in vertices)
        {
            foreach (var e in graph.GetEdgesFrom(v))
            {
                yield return (v, e);
            }
        }
    }

    public static IEnumerable<TVertex> DepthFirst<TVertex, TValue>(this IGraph<TVertex, TValue> graph, TVertex start) where TVertex : IEquatable<TVertex>
    {
        var stack = new Stack<TVertex>();
        stack.Push(start);
        var visited = new HashSet<TVertex>{ start };
        while (stack.Any())
        {
            var current = stack.Pop();
            foreach (TVertex v in graph.GetAdjacentVertices(current))
            {
                if (!visited.Contains(v))
                {
                    stack.Push(v);
                }
                visited.Add(v);
            }
            yield return current;
        }
    }

    public static IEnumerable<TVertex> BreadthFirst<TVertex, TValue>(this IGraph<TVertex, TValue> graph, TVertex start) where TVertex : IEquatable<TVertex>
    {
        var queue = new Queue<TVertex>();
        queue.Enqueue(start);
        var visited = new HashSet<TVertex> { start };

        while (queue.Any())
        {
            var current = queue.Dequeue();
            foreach (TVertex v in graph.GetAdjacentVertices(current))
            {
                if (!visited.Contains(v))
                {
                    queue.Enqueue(v);
                }
                visited.Add(v);
            }
            yield return current;
        }
    }

   
}
