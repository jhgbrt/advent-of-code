using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var example = (
    from line in File.ReadLines("example.txt") select long.Parse(line)
    ).ToArray();

Debug.Assert(example.InvalidNumbers(5).First() == 127);
Debug.Assert(example.FindEncryptionWeakness(127) == 62);

var input = (
    from line in File.ReadLines("input.txt") select long.Parse(line)
    ).ToArray();

var part1 = input.InvalidNumbers(25).First();
var part2 = input.FindEncryptionWeakness(part1);
Console.WriteLine((part1, part2));

static class AoC
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