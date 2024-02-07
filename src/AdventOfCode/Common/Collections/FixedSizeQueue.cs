namespace AdventOfCode.Common.Collections;

public class FixedSizedQueue<T>(int limit)
{
    readonly Queue<T> q = new(limit);
    public void Enqueue(T obj)
    {
        q.Enqueue(obj);
        while (q.Count > limit && q.TryDequeue(out _)) ;
    }
    public int Count => q.Count;
    public T Peek() => q.Peek();
}
