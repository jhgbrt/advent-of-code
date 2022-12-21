using System.Numerics;

namespace AdventOfCode.Common;

static class LinqExtensions
{
    public static IEnumerable<(T a, T? b)> Chunked2<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var a = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                yield return (a, default);
                yield break;
            }
            var b = enumerator.Current;
            yield return (a, b);
        }
    }
    public static IEnumerable<(T a, T? b, T? c)> Chunked3<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var a = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                yield return (a, default, default);
                yield break;
            }
            var b = enumerator.Current;
            if (!enumerator.MoveNext())
            {
                yield return (a, b, default);
                yield break;
            }
            var c = enumerator.Current;
            yield return (a, b, c);
        }
    }

    public static IEnumerable<T> Concat<T>(this IEnumerable<T> list, T item)
    {
        foreach (var entry in list)
            yield return entry;
        yield return item;
    }

    public static IEnumerable<T> AsEnumerable<T>(this T item)
    {
        yield return item;
    }

    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
    }

    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b, T c) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
        yield return tuple.c;

    }
    public static IEnumerable<(T a, T b)> Windowed2<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var b = enumerator.Current;
            yield return (a, b);
            a = b;
        }
    }
    public static IEnumerable<(T a, T b, T c)> Windowed3<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var b = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var c = enumerator.Current;
            yield return (a, b, c);
            (a, b) = (b, c);
        }
    }
    public static IEnumerable<(T a, T b, T c, T d)> Windowed4<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var b = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var c = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var d = enumerator.Current;
            yield return (a, b, c, d);
            (a, b, c) = (b, c, d);
        }
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

    public static (T a, T b) ToTuple2<T>(this T[] items) => items.Length == 2 
        ? ((T a, T b))(items[0], items[1])
        : throw new ArgumentException("Expected 2 items in array");
    public static (T a, T b, T c) ToTuple3<T>(this T[] items) => items.Length == 3 
        ? ((T a, T b, T c))(items[0], items[1], items[2]) 
        : throw new ArgumentException("Expected 3 items in array");
    public static (T a, T b, T c, T d) ToTuple4<T>(this T[] items) => items.Length == 4 
        ? ((T a, T b, T c, T d))(items[0], items[1], items[2], items[3]) 
        : throw new ArgumentException("Expected 4 items in array");
    public static (T a, T b, T c, T d, T e) ToTuple5<T>(this T[] items) => items.Length == 5
        ? ((T a, T b, T c, T d, T e))(items[0], items[1], items[2], items[3], items[4])
        : throw new ArgumentException("Expected 5 items in array");
    public static (T a, T b, T c, T d, T e, T f) ToTuple6<T>(this T[] items) => items.Length == 6
        ? ((T a, T b, T c, T d, T e, T f))(items[0], items[1], items[2], items[3], items[4], items[5])
        : throw new ArgumentException("Expected 6 items in array");
    public static (T a, T b, T c, T d, T e, T f, T g) ToTuple7<T>(this T[] items) => items.Length == 7
        ? ((T a, T b, T c, T d, T e, T f, T g))(items[0], items[1], items[2], items[3], items[4], items[5], items[6])
        : throw new ArgumentException("Expected 7 items in array");

    public static T Max<T>(this (T item1, T item2) tuple) where T : INumber<T>
        => T.Max(tuple.item2, tuple.item2);
    public static T Max<T>(this (T item1, T item2, T item3) tuple) where T : INumber<T> 
        => T.Max(tuple.item1, T.Max(tuple.item2, tuple.item3));
    public static T Max<T>(this (T item1, T item2, T item3, T item4) tuple) where T : INumber<T> 
        => T.Max(tuple.item1, T.Max(tuple.item2, T.Max(tuple.item3, tuple.item4)));
    public static T Max<T>(this (T item1, T item2, T item3, T item4, T item5) tuple) where T : INumber<T>
        => T.Max(tuple.item1, T.Max(tuple.item2, T.Max(tuple.item3, T.Max(tuple.item4, tuple.item5))));

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> node) => node.Previous ?? node?.List?.Last ?? throw new Exception("Inconsistent linked list");
    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> node) => node.Next ?? node?.List?.First ?? throw new Exception("Inconsistent linked list");


}
