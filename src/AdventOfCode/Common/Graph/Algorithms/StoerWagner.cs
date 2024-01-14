using Net.Code.Graph.Collections;

namespace Net.Code.Graph.Algorithms;

public record MinCutResult<TVertex>(int MinCutWeight, IReadOnlySet<TVertex> Partition1, IReadOnlySet<TVertex> Partition2, IReadOnlySet<Edge<TVertex, int>> EdgesOnCut);
public static class StoerWagner
{
    // adapted from https://github.com/chriswaters78/AdventOfCode2023/blob/main/2023_25/StoerWagner.cs
    public static MinCutResult<TVertex> MinimumCut<TVertex>(IGraph<TVertex, int> graph)
    where TVertex : IEquatable<TVertex>, IComparable<TVertex>
    {
        var d = graph.ToDictionary();
        var (minCut, partition) = MinimumCut(d);
        var partition1 = partition.Select(graph.GetVertex).ToHashSet();
        var partition2 = graph.Vertices.Except(partition1).ToHashSet();
        var edgesOnCut = from e in graph.Edges
                         where (!partition1.Contains(e.Source) && !partition2.Contains(e.Destination))
                         || (!partition2.Contains(e.Source) && !partition1.Contains(e.Destination))
                         select e;
        return new(minCut, partition1, partition2, edgesOnCut.ToHashSet());
    }

    static (int minCut, List<int> partition) MinimumCut(Dictionary<int, List<(int dest, int weight)>> graph)
    {
        var a = graph.Keys.First();
        (int minCut, List<int> partition) = (int.MaxValue, []);

        var g = graph.ToDictionary(kvp => kvp.Key, kvp => ((List<int>)[kvp.Key], kvp.Value.ToDictionary(x => x.dest, x => x.weight)));
        while (g.Count > 1)
        {
            // for a given starting node, it finds the most leasely connected vertex
            // and merges that with the starting node
            // keep doing this until we have merged the entire graph
            // the least tightly connected vertex we add is our minimum cut
            var phaseResult = MinimumCutPhase(g, a);
            if (phaseResult.minCut < minCut)
                (minCut, partition) = phaseResult;
        }

        return (minCut, partition);
    }

    static (int minCut, List<int> partition) MinimumCutPhase(Dictionary<int, (List<int> merges, Dictionary<int, int> edges)> graph, int a)
    {
        var A = new List<int>(graph.Count) { a };

        var weights = new FibonacciHeap<int, int>(HeapDirection.Decreasing);

        var cells = (
            from v in graph.Keys
            where v != a
            let edges = graph[v].edges
            select edges.ContainsKey(a) ? (priority: edges[a], v) : (priority: 0, v)
        ).ToDictionary(x => x.v, x => weights.Enqueue(x.priority, x.v));

        int cutOfPhase = int.MaxValue;

        while (A.Count != graph.Count)
        {
            // find the most strongly connected vertex to A
            (cutOfPhase, var next) = weights.Dequeue();
            cells.Remove(next);
            A.Add(next);

            // Update weights. Every vertex connected to next that is not already merged
            // must have its priority increased by the edge weight as these are now directly
            // connected to A and are candidate for selection
            foreach ((var connected, var count) in graph[next].edges)
            {
                if (cells.ContainsKey(connected))
                {
                    weights.ChangeKey(cells[connected], cells[connected].Priority + count);
                }
            }
        }

        // last node added represents mininum cut for this phase
        // so return the list of nodes that were merged into it
        // before we contract it with our existing seed vertex
        var partition = graph[A[^1]].merges;

        // Merging the last two added nodes, this is the least tightly connected vertex
        // and so maintains the property that all we know the min cut of all nodes edge-contracted with a
        ContractEdge(graph, A[^2], A[^1]);

        return (cutOfPhase, partition);
    }

    static void ContractEdge(Dictionary<int, (List<int> merges, Dictionary<int, int> edges)> graph, int v1, int v2)
    {
        // merge everything into v1 and keep that key
        // and record any merges in the merges list for that node
        // remove the edge between v1 and v2 and record the merge history
        graph[v1].edges.Remove(v2);
        graph[v2].edges.Remove(v1);
        graph[v1].merges.AddRange(graph[v2].merges);

        // for every edge that did exist between v2 and v3
        foreach ((var v3, var count) in graph[v2].edges)
        {
            if (graph[v1].edges.ContainsKey(v3))
            {
                graph[v1].edges[v3] += count;
                graph[v3].edges[v1] += count;
            }
            else
            {
                graph[v1].edges[v3] = count;
                graph[v3].edges[v1] = count;
            }
            graph[v3].edges.Remove(v2);
        }

        graph.Remove(v2);
    }
}
