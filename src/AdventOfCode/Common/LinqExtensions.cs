namespace AdventOfCode.Common;

static class LinqExtensions
{
    public static IEnumerable<(T a, T? b)> PairWise<T>(this IEnumerable<T> list)
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
    public static IEnumerable<(T a, T? b, T? c)> TripletWise<T>(this IEnumerable<T> list)
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
}