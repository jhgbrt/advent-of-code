
namespace AdventOfCode;

public static class Dijkstra
{
    /// <summary>
    /// Uses the Dijkstra Search Algorithm to find from one Vertex to another in the given Graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static Vertex[] Search(Graph g, Vertex from, Vertex to)
    {
        //Initialize
        Dictionary<Vertex, int> distances = new Dictionary<Vertex, int>();
        Dictionary<Vertex, Vertex> predecessor = new Dictionary<Vertex, Vertex>();

        bool found = false;

        distances.Add(from, 0);
        predecessor.Add(from, from);

        foreach (Vertex v in g.Vertices)
        {
            if (!v.Equals(from))
            {
                distances.Add(v, Int32.MaxValue);
                predecessor.Add(v, null);
            }
        }
        PriorityQueue<Vertex> q = new PriorityQueue<Vertex>();

        q.Insert(from, 0);

        //Algorithm
        while (q.Count != 0)
        {
            Vertex u = q.PopLowest();

            //Goal Vertex found
            if (u.Equals(to))
            {
                found = true;
                break;
            }

            //Find all neighbors of u
            Vertex[] neighbors = GraphHelper.FindAdjacentVertices(g, u);

            //Analyse the neighbors and update their distances and predecessors accordingly
            foreach (Vertex v in neighbors)
            {
                if (predecessor[v] != null)
                {
                    //update distance
                    if (distances[u] + 1 < distances[v])
                    {
                        distances[v] = distances[u] + 1;
                        predecessor[v] = u;
                        q.InsertOverride(v, distances[u] + 1);
                    }
                }
                else
                {
                    distances[v] = distances[u] + 1;
                    predecessor[v] = u;
                    q.InsertOverride(v, distances[u] + 1);
                }
            }
        }

        if (found)
            return GetPath(predecessor, to);
        else
            return null;
    }

    //Returns a sorted array of vertices that represent the path from the start to the goal
    private static Vertex[] GetPath(Dictionary<Vertex, Vertex> p, Vertex to)
    {
        List<Vertex> result = new List<Vertex>();

        Vertex next = to;
        result.Insert(0, next);

        while (next != p[next])
        {
            result.Insert(0, p[next]);
            next = p[next];
        }

        return result.ToArray();
    }

    /// <summary>
    /// Returns the number of reachable nodes from the given vertex inside the given graph.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="from"></param>
    /// <returns></returns>
    public static int CountReachableNodes(Graph g, Vertex from)
    {
        //Initialize
        Dictionary<Vertex, int> distances = new Dictionary<Vertex, int>();
        Dictionary<Vertex, Vertex> predecessor = new Dictionary<Vertex, Vertex>();

        int count = 1;

        distances.Add(from, 0);
        predecessor.Add(from, from);

        foreach (Vertex v in g.Vertices)
        {
            if (!v.Equals(from))
            {
                distances.Add(v, Int32.MaxValue);
                predecessor.Add(v, null);
            }
        }
        PriorityQueue<Vertex> q = new PriorityQueue<Vertex>();

        q.Insert(from, 0);

        //Algorithm
        while (q.Count != 0)
        {
            //Get next Vertex
            Vertex u = q.PopLowest();

            //Find all neighbors of u
            Vertex[] neighbors = GraphHelper.FindAdjacentVertices(g, u);

            //Analyse the neighbors and update their distances and predecessors accordingly
            foreach (Vertex v in neighbors)
            {
                if (predecessor[v] != null)
                {
                    //update distance
                    if (distances[u] + 1 < distances[v])
                    {
                        distances[v] = distances[u] + 1;
                        predecessor[v] = u;
                        q.InsertOverride(v, distances[u] + 1);
                    }
                }
                else
                {
                    distances[v] = distances[u] + 1;
                    predecessor[v] = u;
                    q.InsertOverride(v, distances[u] + 1);
                    count++;
                }
            }
        }
        return count;
    }
}


/// <summary>
/// Represents a single edge between two vertices inside a graph.
/// </summary>
public record struct Edge
{
    private Pair<Vertex> _vertices;

    public Pair<Vertex> Vertices
    {
        get {
            return _vertices;
        }
        set {
            _vertices = Vertices;
        }
    }

    public Edge(Vertex a, Vertex b)
    {
        _vertices = new Pair<Vertex>(a, b);
    }
    public Edge(Pair<string> p)
    {
        _vertices = new Pair<Vertex>(new Vertex(p.a), new Vertex(p.b));
    }

    #region Overrides
    public override string ToString()
    {
        return _vertices.a + " - " + _vertices.b;
    }

    #endregion
}

/// <summary>
/// A class that handles all algorithms and data structures needed when using graphs.
/// </summary>
public class Graph
{

    #region Instance Variables

    //vertices should have quick access through usage of a hashmap
    private Dictionary<Vertex, bool> _vertices;
    private Dictionary<Edge, bool> _edges;

    public Vertex[] Vertices
    {
        get {
            return _vertices.Keys.ToArray<Vertex>();
        }
    }
    public Edge[] Edges
    {
        get {
            return _edges.Keys.ToArray<Edge>();
        }
    }

    #endregion


    #region Constructors
    /// <summary>
    /// Creates an empty Graph.
    /// </summary>
    public Graph()
    {
        _vertices = new Dictionary<Vertex, bool>();
        _edges = new Dictionary<Edge, bool>();
    }

    public Graph(Vertex[] vertices, Edge[] edges)
    {
        _vertices = new Dictionary<Vertex, bool>();
        _edges = new Dictionary<Edge, bool>();

        foreach (Vertex v in vertices)
        {
            AddNewVertex(v);
        }
        foreach (Edge e in edges)
        {
            AddEdge(e);
        }
    }

    public Graph(string[] vertices, Edge[] edges)
    {
        _vertices = new Dictionary<Vertex, bool>();
        _edges = new Dictionary<Edge, bool>();

        foreach (string v in vertices)
        {
            AddVertex(v);
        }
        foreach (Edge e in edges)
        {
            AddEdge(e);
        }
    }

    public Graph(Vertex[] vertices, Pair<string>[] edges)
    {
        _vertices = new Dictionary<Vertex, bool>();
        _edges = new Dictionary<Edge, bool>();

        foreach (Vertex v in vertices)
        {
            AddNewVertex(v);
        }
        foreach (Pair<string> e in edges)
        {
            AddNewEdge(e);
        }
    }

    public Graph(string[] vertices, Pair<string>[] edges)
    {
        _vertices = new Dictionary<Vertex, bool>();
        _edges = new Dictionary<Edge, bool>();

        foreach (string v in vertices)
        {
            AddVertex(v);
        }
        foreach (Pair<string> e in edges)
        {
            AddNewEdge(e);
        }
    }

    #endregion


    #region Add/Remove-Methods

    /// <summary>
    /// Adds a new Vertex with the specified name.
    /// Returns false when Vertex already existed.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool AddVertex(string n)
    {
        return AddNewVertex(new Vertex(n));
    }

    /// <summary>
    /// Adds the specified Vertex to the Graph.
    /// Returns false when Vertex already existed.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public bool AddNewVertex(Vertex v)
    {
        if (!_vertices.ContainsKey(v))
        {
            _vertices.Add(v, true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds a new Edge between the two vertices defined by the strings.
    /// Returns false when edge already existed.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool AddNewEdge(string a, string b)
    {
        return AddNewEdge(new Pair<string>(a, b));
    }

    /// <summary>
    /// Adds a new Edge between the two vertices defined by the string-pair.
    /// Returns false when edge already existed.
    /// </summary>
    /// <param name="stringpair"></param>
    /// <returns></returns>
    public bool AddNewEdge(Pair<string> p)
    {
        return AddEdge(new Edge(p));
    }

    /// <summary>
    /// Adds the specified Edge to the Graph.
    /// Returns false when edge already existed.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool AddEdge(Edge newEdge)
    {
        if (!_edges.ContainsKey(newEdge))
        {
            Vertex firstV = newEdge.Vertices.a;
            Vertex lastV = newEdge.Vertices.b;
            if (!_vertices.ContainsKey(firstV))
            {
                AddNewVertex(firstV);
            }
            if (!_vertices.ContainsKey(lastV))
            {
                AddNewVertex(lastV);
            }
            _edges.Add(newEdge, true);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the defined Edge from the Graph.
    /// Returns false when the Edge did not exist.
    /// </summary>
    /// <param name="edge"></param>
    /// <returns></returns>
    public bool RemoveEdge(Edge edge)
    {
        if (_edges.ContainsKey(edge))
        {
            _edges.Remove(edge);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Removes the defined Vertex and all Edges containing it from the Graph.
    /// Returns false when the Vertex did not exist.
    /// </summary>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public bool RemoveVertex(Vertex vertex)
    {
        if (!_vertices.ContainsKey(vertex))
        {
            return false;
        }
        foreach (Edge edge in _edges.Keys)
        {
            if (edge.Vertices.Contains(vertex))
                _edges.Remove(edge);
        }
        _vertices.Remove(vertex);
        return true;
    }

    #endregion
    public bool IsVertexReachableFrom(Vertex start, Vertex goal) => Dijkstra.Search(this, start, goal) != null;

    public bool IsComplete()
    {

        foreach (Vertex v in this.Vertices)
        {
            List<Vertex> neighbors = GraphHelper.FindAdjacentVertices(this, v).ToList<Vertex>();
            neighbors.Add(v);

            if (!this.Vertices.All(neighbors.Contains))
                return false;
        }

        return true;
    }

    public bool IsBipartite()
    {
        Dictionary<Vertex, bool> U = [];
        Dictionary<Vertex, bool> V = [];
        Vertex src = Vertices[0];

        var q = new Queue<Vertex>();
        q.Enqueue(src);

        U.Add(src, true);

        while (q.Count != 0)
        {
            Vertex v = q.Dequeue();
            foreach (Vertex vn in GraphHelper.FindAdjacentVertices(this, v))
            {
                if (!U.ContainsKey(vn) && !V.ContainsKey(vn))
                {
                    if (U.ContainsKey(v))
                        V.Add(vn, true);
                    else
                        U.Add(vn, true);
                    q.Enqueue(vn);
                }
                else if ((U.ContainsKey(v) && U.ContainsKey(vn)) || (V.ContainsKey(v) && V.ContainsKey(vn)))
                    return false;
            }
        }
        return true;
    }

    public bool IsConnected() => Dijkstra.CountReachableNodes(this, Vertices[0]) == Vertices.Length;
}

public record struct Pair<T>(T a, T b)
{
    public bool Contains(T item) => item is not null && (item.Equals(a) || item.Equals(b));

    public T? GetOther(T item) => item switch
    {
        not null when item.Equals(a) => b,
        not null when item.Equals(b) => a,
        _ => default
    };
}

/// <summary>
/// Represents a single vertex inside a graph.
/// </summary>
public record Vertex(string Name);


/// <summary>
/// Holds several static Methods that may help when using Graphs.
/// </summary>
public static class GraphHelper
{

    #region FindAdjacent
    /// <summary>
    /// Returns an array of all Edges that are adjacent to the given vertex in the graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public static Edge[] FindAdjacentEdges(Graph g, Vertex v)
    {
        List<Edge> result = new List<Edge>();

        foreach (Edge e in g.Edges)
        {
            if (e.Vertices.Contains(v))
                result.Add(e);
        }

        return result.ToArray();
    }

    /// <summary>
    /// Returns an array of all Edges that are adjacent to one of the vertices in the specified edge inside the graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Edge[] FindAdjacentEdges(Graph g, Edge e)
    {
        List<Edge> result = new List<Edge>();

        result.AddRange(FindAdjacentEdges(g, e.Vertices.a));
        result.AddRange(FindAdjacentEdges(g, e.Vertices.b));

        return result.ToArray();
    }

    /// <summary>
    /// Returns an array of all vertices that are adjacent to the specified vertex inside the graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="vertex"></param>
    /// <returns></returns>
    public static Vertex[] FindAdjacentVertices(Graph g, Vertex v)
    {
        List<Vertex> result = new List<Vertex>();

        foreach (Edge e in g.Edges)
        {
            if (e.Vertices.Contains(v))
                result.Add(e.Vertices.GetOther(v));
        }

        return result.ToArray();
    }

    /// <summary>
    /// Returns an array of all vertices that are adjacent to one of the vertices of the specified edge inside the graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="edge"></param>
    /// <returns></returns>
    public static Vertex[] FindAdjacentVertices(Graph g, Edge e)
    {
        List<Vertex> result = new List<Vertex>();

        result.AddRange(FindAdjacentVertices(g, e.Vertices.a));
        result.AddRange(FindAdjacentVertices(g, e.Vertices.b));

        return result.ToArray();
    }
    #endregion

}



public class PriorityQueue<T>
{
    Dictionary<T, int> _priority;
    List<T> _queue;

    public PriorityQueue()
    {
        _priority = new Dictionary<T, int>();
        _queue = new List<T>();
    }

    public int Count
    {
        get {
            return _queue.Count;
        }
    }

    /// <summary>
    /// Adds the item with the given priority to the Queue.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="priority"></param>
    public void Insert(T t, int i)
    {
        if (_queue.Count == 0)
        {
            _queue.Insert(0, t);
        }
        else
        {
            bool positioned = false;
            int k = 0;

            while (!positioned)
            {
                T next = _queue[k];

                if (_priority[next] > i)
                {
                    _queue.Insert(k, t);
                    positioned = true;
                }

                k++;

                if (k > _queue.Count - 1)
                {
                    _queue.Add(t);
                    positioned = true;
                }
            }
        }
        _priority.Add(t, i);
    }

    public void InsertOverride(T t, int i)
    {
        if (_priority.ContainsKey(t))
        {
            _priority.Remove(t);
            _queue.Remove(t);
        }
        Insert(t, i);
    }

    /// <summary>
    /// Returns the item in the List with the lowest priority.
    /// </summary>
    /// <returns></returns>
    public T PeekLowest()
    {
        return _queue[0];
    }

    /// <summary>
    /// Returns the item in the Queue with the highest priority.
    /// </summary>
    /// <returns></returns>
    public T PeekHighest()
    {
        return _queue[_queue.Count - 1];
    }

    /// <summary>
    /// Returns and removes the item in the Queue with the lowest priority.
    /// </summary>
    /// <returns></returns>
    public T PopLowest()
    {
        T result = _queue[0];

        _queue.RemoveAt(0);
        _priority.Remove(result);

        return result;
    }

    /// <summary>
    /// Returns and removes the item in the Queue with the highest priority.
    /// </summary>
    /// <returns></returns>
    public T PopHighest()
    {
        int i = _queue.Count - 1;

        T result = _queue[i];

        _queue.RemoveAt(i);
        _priority.Remove(result);

        return result;
    }

    /// <summary>
    /// Returns true when the PriorityQueue contains the given item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        return _priority.ContainsKey(item);
    }
}