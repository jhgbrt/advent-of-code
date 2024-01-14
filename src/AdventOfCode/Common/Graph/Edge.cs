namespace Net.Code.Graph;

public static class Edge
{
    public static Edge<TVertex, TValue> Create<TVertex, TValue>(TVertex source, TVertex destination, TValue value) => new(source, destination, value);
}

public record struct Edge<TVertex, TValue>(TVertex Source, TVertex Destination, TValue Value)
{
    public static implicit operator Edge<TVertex, TValue>((TVertex Source, TVertex Destination, TValue Value) tuple) => new(tuple.Source, tuple.Destination, tuple.Value);
    public override string ToString()
    {
        return $"{Source} -({Value})-> {Destination}";
    }
}

public static class Vertex
{
    public static Vertex<TVertex> Create<TVertex>(TVertex value) => new(value);
}

public readonly record struct Vertex<TVertex>(TVertex Value)
{
    public static implicit operator Vertex<TVertex>(TVertex value) => new(value);
}

public static class TupleExtensions
{
    public static (T, T) Ordered<T>(this (T, T) tuple) where T : IComparable<T>
    => tuple.Item1.CompareTo(tuple.Item2) >= 0
        ? tuple
        : (tuple.Item2, tuple.Item1);
}
