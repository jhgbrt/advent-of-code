
using static AdventOfCode.Year2020.Day09.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day09
{
    partial class AoC
    {
        static long[] input = File.ReadLines("input.txt").Select(long.Parse).ToArray();

        internal static Result Part1() => Run(() => input.InvalidNumbers(25).First());
        internal static Result Part2() => Run(() => input.FindEncryptionWeakness((long)Part1().Value));

    }
}

public class Tests
{
    static long[] input = File.ReadLines("example.txt").Select(long.Parse).ToArray();
    public void Test1() => Assert.Equal(127, input.InvalidNumbers(5).First());
    public void Test2() => Assert.Equal(62, input.FindEncryptionWeakness(127));
}


static class Ex
{
    public static long FindEncryptionWeakness(this long[] array, long invalid)
        => (
            from i in Enumerable.Range(0, array.Length)
            from p in (
                from j in Enumerable.Range(i + 1, array.Length - i)
                select (j, sum: array.Skip(i).Take(j - i).Sum())
                ).TakeWhile(p => p.sum <= invalid)
            where p.sum == invalid
            let range = array[i..p.j]
            select range.Min() + range.Max()
            ).First();

    public static IEnumerable<long> InvalidNumbers(this IEnumerable<long> input, int preamble)
    {
        var enumerator = input.GetEnumerator();

        Queue<long> q = new();
        for (int i = 0; i < preamble; i++)
        {
            if (!enumerator.MoveNext()) break;
            var current = enumerator.Current;
            q.Enqueue(current);
        }

        while (enumerator.MoveNext())
        {
            var set = q.PairWiseSums();
            var current = enumerator.Current;
            if (!set.Contains(current))
                yield return current;
            q.Dequeue();
            q.Enqueue(enumerator.Current);
        }
    }

    public static IEnumerable<long> PairWiseSums(this IEnumerable<long> input)
    {
        var set = new HashSet<long>();
        foreach (var number in input)
        {
            if (!set.Contains(number))
            {
                foreach (var i in set) yield return number + i;
                set.Add(number);
            }
        }
    }

}