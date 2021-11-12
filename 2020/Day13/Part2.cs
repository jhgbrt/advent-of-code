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
        foreach (var (n,a) in entries)
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
