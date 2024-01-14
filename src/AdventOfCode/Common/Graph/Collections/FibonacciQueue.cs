using System.Diagnostics;

namespace Net.Code.Graph.Collections;


[DebuggerDisplay("Count = {Count}")]
public sealed class FibonacciQueue<TVertex, TDistance> : IPriorityQueue<TVertex>
    where TVertex: notnull where TDistance : struct
{

    public FibonacciQueue(
        IReadOnlyDictionary<TVertex, TDistance> distances,
        Func<TDistance, TDistance, int> distanceComparison
        )
    {
        this.distances = distances;
        this.cells = (from kv in distances
                     let key = kv.Key
                     let value = new FibonacciHeapCell<TDistance, TVertex>(kv.Value, kv.Key)
                     {
                         Removed = true
                     }
                     select(key,value)).ToDictionary(t => t.key, t => t.value);

        heap = new (
            HeapDirection.Increasing, distanceComparison
            );
    }

    public FibonacciQueue(
        Dictionary<TVertex, TDistance> distances)
        : this(distances, Comparer<TDistance>.Default.Compare)
    { }

    private readonly FibonacciHeap<TDistance, TVertex> heap;
    private readonly Dictionary<TVertex, FibonacciHeapCell<TDistance, TVertex>> cells;
    private readonly IReadOnlyDictionary<TVertex, TDistance> distances;

    public int Count => heap.Count;

    public bool Contains(TVertex value) => cells.TryGetValue(value, out var result) && !result.Removed;

    public void Update(TVertex v)
    {
        heap.ChangeKey(cells[v], distances[v]);
    }

    public void Enqueue(TVertex value)
    {
        cells[value] = heap.Enqueue(distances[value], value);
    }

    public TVertex Dequeue() => heap.Dequeue().Value;

    public TVertex Peek() { return heap.Top is null ? throw new InvalidOperationException("Queue is empty") : heap.Top.Value; }

    public TVertex[] ToArray() => heap.Select(e => e.Value).ToArray();
}
