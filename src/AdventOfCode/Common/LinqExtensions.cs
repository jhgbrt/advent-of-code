namespace AdventOfCode;

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
    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b, T c, T d) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
        yield return tuple.c;
        yield return tuple.d;
    }
    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b, T c, T d, T e) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
        yield return tuple.c;
        yield return tuple.d;
        yield return tuple.e;
    }
    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b, T c, T d, T e, T f) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
        yield return tuple.c;
        yield return tuple.d;
        yield return tuple.e;
        yield return tuple.f;
    }
    public static IEnumerable<T> AsEnumerable<T>(this (T a, T b, T c, T d, T e, T f, T g) tuple)
    {
        yield return tuple.a;
        yield return tuple.b;
        yield return tuple.c;
        yield return tuple.d;
        yield return tuple.e;
        yield return tuple.f;
        yield return tuple.g;
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

    public static IEnumerable<(T a, T b, T c, T d, T e)> Windowed5<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var b = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var c = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var d = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var e = enumerator.Current;
            yield return (a, b, c, d, e);
            (a, b, c, d) = (b, c, d, e);
        }
    }

    public static IEnumerable<(T a, T b, T c, T d, T e, T f)> Windowed6<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var b = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var c = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var d = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var e = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var f = enumerator.Current;
            yield return (a, b, c, d, e, f);
            (a, b, c, d, e) = (b, c, d, e, f);
        }
    }

    public static IEnumerable<(T a, T b, T c, T d, T e, T f, T g)> Windowed7<T>(this IEnumerable<T> list)
    {
        var enumerator = list.GetEnumerator();
        if (!enumerator.MoveNext()) yield break;
        var a = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var b = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var c = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var d = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var e = enumerator.Current;
        if (!enumerator.MoveNext()) yield break;
        var f = enumerator.Current;
        while (true)
        {
            if (!enumerator.MoveNext()) yield break;
            var g = enumerator.Current;
            yield return (a, b, c, d, e, f, g);
            (a, b, c, d, e, f) = (b, c, d, e, f, g);
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

    public static T Max<T>(this (T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;
    public static T Max<T>(this (T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;
    public static T Max<T>(this (T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;
    public static T Max<T>(this (T, T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;
    public static T Max<T>(this (T, T, T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;
    public static T Max<T>(this (T, T, T, T, T , T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Max()!;

    public static T Min<T>(this (T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;
    public static T Min<T>(this (T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;
    public static T Min<T>(this (T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;
    public static T Min<T>(this (T, T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;
    public static T Min<T>(this (T, T, T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;
    public static T Min<T>(this (T, T, T, T, T, T, T) tuple) where T : INumber<T>
        => tuple.AsEnumerable().Min()!;

    public static LinkedListNode<T> PreviousOrLast<T>(this LinkedListNode<T> node) => node.Previous ?? node?.List?.Last ?? throw new Exception("Inconsistent linked list");
    public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> node) => node.Next ?? node?.List?.First ?? throw new Exception("Inconsistent linked list");


}
