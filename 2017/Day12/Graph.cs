using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Plumber
{
    class Graph
    {
        private readonly ImmutableList<(int vertex1, int vertex2)> _edges;

        public Graph(IEnumerable<(int, int)> edges)
        {
            _edges = edges.ToImmutableList();
        }

        public IReadOnlyDictionary<int,IReadOnlyCollection<int>> SubGraphs()
        {
            var parents = _edges.ToLookup(x => x.vertex2, x => x.vertex1);
            var subgraphs = ImmutableDictionary<int, IReadOnlyCollection<int>>.Empty;
            foreach (var parent in _edges.Select(x => x.vertex1))
            {
                if (subgraphs.Any(nodes => nodes.Value.Contains(parent))) continue;
                var subgraph = new[] { parent }.ToImmutableHashSet();
                int count;
                do
                {
                    count = subgraph.Count;
                    subgraph = subgraph.Union(subgraph.SelectMany(c => parents[c]));
                } while (subgraph.Count > count);
                subgraphs = subgraphs.Add(parent, subgraph);
            }
            return subgraphs;

        }

        static void ForEach(ILookup<int, int> nodes, int start, Action<int> action)
        {
            var hashSet = new HashSet<int>();
            VisitAll(nodes, new[] { start }, hashSet, action);
        }
        static void VisitAll(ILookup<int, int> nodes, IEnumerable<int> start, ISet<int> visited, Action<int> action)
        {
            foreach (int v in start)
            {
                if (visited.Contains(v)) continue;
                visited.Add(v);
                action(v);
                VisitAll(nodes, nodes[v], visited, action);
            }
        }

        public int Count(int i)
        {
            var children = _edges.ToLookup(x => x.vertex1, x => x.vertex2);
            int n = 0;
            ForEach(children, 0, _ => n++);
            return n;

        }
    }
}