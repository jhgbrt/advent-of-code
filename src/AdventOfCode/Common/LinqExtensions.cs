﻿namespace AdventOfCode.Common;

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
}