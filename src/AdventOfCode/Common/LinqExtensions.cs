using System.Collections.Concurrent;

namespace AdventOfCode;

static class LinqExtensions
{
    public static IEnumerable<T> Except<T>(this IEnumerable<T> list, T item) => list.Except(item.AsEnumerable());
    public static IEnumerable<T> AsEnumerable<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<IReadOnlyList<T>> Windowed<T>(this IEnumerable<T> list, int size)
    {
        var buffer = new Queue<T>();

        foreach (var item in list)
        {
            buffer.Enqueue(item);
            if (buffer.Count == size)
            {
                yield return buffer.ToArray();
                buffer.Dequeue();
            }
        }
    }

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> node) => node.Previous ?? node?.List?.Last ?? throw new Exception("Inconsistent linked list");
    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> node) => node.Next ?? node?.List?.First ?? throw new Exception("Inconsistent linked list");
    public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> list, int length) => length == 1
        ? list.Select(t => t.AsEnumerable())
        : from t in GetPermutations(list, length - 1)
          from e in list.Except(t)
          select t.Append(e);

    public static T Product<T>(this IEnumerable<T> list) where T : INumber<T>
        => list.Aggregate(T.MultiplicativeIdentity, (a, b) => a * b);

}
public static class Memoizer
{
    public static Func<T, TResult> Memoize<T, TResult>(this Func<T, TResult> f)
    {
        var cache = new ConcurrentDictionary<T, TResult>();
        return a => cache.GetOrAdd(a, f);
    }
}