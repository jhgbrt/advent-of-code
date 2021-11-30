
using Blocks = System.Collections.Generic.IEnumerable<System.Collections.Generic.IEnumerable<string>>;

namespace AdventOfCode.Year2020.Day06;

static class Ex
{
    internal static Blocks AsBlocks(this IEnumerable<string> lines)
    {
        var enumerator = lines.GetEnumerator();
        while (enumerator.MoveNext())
            yield return GetBlock(enumerator);
    }
    private static IEnumerable<string> GetBlock(IEnumerator<string> enumerator)
    {
        while (!string.IsNullOrEmpty(enumerator.Current))
        {
            yield return enumerator.Current;
            if (!enumerator.MoveNext()) break;
        }
    }

}