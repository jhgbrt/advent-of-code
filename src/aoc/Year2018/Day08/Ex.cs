namespace AdventOfCode.Year2018.Day08;

static class Ex
{
    public static T Next<T>(this IEnumerator<T> enumerator)
    {
        enumerator.MoveNext();
        return enumerator.Current;
    }

    public static IEnumerable<T> Read<T>(this IEnumerator<T> enumerator, int n)
        => Enumerable.Range(0, n).Select(i => enumerator.Next());
    public static IEnumerable<int> ToIntegers(this string input)
    {
        var sb = new StringBuilder();
        foreach (var c in input)
        {
            if (char.IsDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                yield return int.Parse(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0)
            yield return int.Parse(sb.ToString());
    }

}