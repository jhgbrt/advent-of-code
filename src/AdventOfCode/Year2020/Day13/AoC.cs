
using P1 = AdventOfCode.Year2020.Day13.Part1;
using P2 = AdventOfCode.Year2020.Day13.Part2;

namespace AdventOfCode.Year2020.Day13;

public class AoC202013
{
    public object Part1() => P1.Run();
    public object Part2() => P2.Run();
}

class Part1
{
    public static int Run()
    {

        var input = @"1000434
17,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,983,x,29,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,23,x,x,x,x,x,x,x,397,x,x,x,x,x,37,x,x,x,x,x,x,13";

        using var r = new StringReader(input);
        var start = int.Parse(r.ReadLine()!);

        var query = from x in r.ReadLine()!.Split(",")
                    where x != "x"
                    let id = int.Parse(x)
                    let timestamp = EnumerableEx.InfiniteSequence(start).First(t => t % id == 0)
                    orderby timestamp ascending
                    select (id, timestamp);
        var result = query.First();

        return (result.id * (result.timestamp - start));

    }
}

static class EnumerableEx
{
    public static IEnumerable<int> InfiniteSequence(int start)
    {
        while (true) yield return start++;
    }
}


static class Part2
{
    public static long Run()
    {
        // background reading:
        // https://mathworld.wolfram.com/Congruence.html
        // https://en.wikipedia.org/wiki/Modular_multiplicative_inverse
        // https://rosettacode.org/wiki/Chinese_remainder_theorem (contains ready-made implementations)

        //var example = @"7,13,x,x,59,x,31,19";
        //var example = @"17,x,13,19";
        var input = @"17,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,983,x,29,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,23,x,x,x,x,x,x,x,397,x,x,x,x,x,37,x,x,x,x,x,x,13";

        var constraints = from entry in input.Split(",").Select((x, i) => (x, i))
                          where entry.x != "x"
                          let n = long.Parse(entry.x)
                          let a = (long)n - entry.i
                          select (n, a);

        return RemainderTheorem.Solve(constraints.ToArray());

    }
}


public static class RemainderTheorem
{
    public static long Solve((long n, long a)[] entries)
    {
        long prod = entries.Aggregate(1L, (r, e) => r * e.n);
        long p;
        long s = 0;
        foreach (var (n, a) in entries)
        {
            p = prod / n;
            s += a * ModularMultiplicativeInverse(p, n) * p;
        }
        return s % prod;
    }
    private static long ModularMultiplicativeInverse(long a, long mod)
    {
        long b = a % mod;
        for (int x = 1; x < mod; x++)
        {
            if (b * x % mod == 1)
            {
                return x;
            }
        }
        return 1;
    }
}
