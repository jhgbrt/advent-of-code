using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2025.Day02;

readonly record struct RepeatedDigits(long start, long end) : IEnumerable<long>
{
    public static RepeatedDigits Parse(string s) 
    {
        var parts = s.Split('-');
        var start = long.Parse(parts[0]);
        var end = long.Parse(parts[1]);
        return new RepeatedDigits(start, end);
    }

    public IEnumerator<long> GetEnumerator()
    {
        var digitsStart = NumDigits(start);
        var digitsEnd = NumDigits(end);

        for (int n = digitsStart; n <= digitsEnd; n++)
        {
            // skip odd digit lengths; only even digit lengths can form N = XX
            if ((n & 1) == 1)   
                continue;

            int k = n / 2;
            long pow10k = Pow10(k);     // e.g. when start = 1234, k = 2, pow10k = 100
            long @base = pow10k / 10;   // smallest k-digit x (e.g. 10 for k=2)
            long max = pow10k - 1;

            // generate candidates: N = x * 10^k + x
            // max = largest  k-digit x (e.g. 99 for k=2)
            long xMin = LowerBoundX(start, pow10k, @base, max: pow10k - 1);
            long xMax = UpperBoundX(end, pow10k, @base, max: pow10k - 1); 

            if (xMin > xMax)
                continue;

            for (long x = xMin; x <= xMax; x++)
            {
                yield return Repeat(x, pow10k);
            }
        }
    }

    // find smallest x such that Repeat(x) = x * 10^k + x = x * (10^k + 1) >= target
    private static long LowerBoundX(long target, long pow10k, long @base, long max)
    {
        // if Repeat(base) is already >= target, return it
        if (Repeat(@base, pow10k) >= target)
            return @base;
        long x = (target + (pow10k + 1) - 1) / (pow10k + 1);
        if (x < @base) return @base;
        if (x > max) return max;
        return x;
    }

    // find largest x such that Repeat(x) = x * 10^k + x = x * (10^k + 1) <= target
    private static long UpperBoundX(long target, long pow10k, long @base, long max)
    {
        long x = target / (pow10k + 1);
        if (x < @base) return @base - 1;
        if (x > max) return max;
        return x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Repeat(long baseVal, long pow10k) => baseVal * pow10k + baseVal;


    private static long Pow10(int exp)
    {
        long result = 1;
        for (int i = 0; i < exp; i++)
        {
            result *= 10;
        }
        return result;
    }

    private int NumDigits(long n)
    {
        var count = 0;
        while (n > 0)
        {
            count++;
            n /= 10;
        }
        return count;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public class AoC202502(string[] input, TextWriter writer)
{
    public AoC202502() : this(Read.InputLines(), Console.Out) {}

    RepeatedDigits[] ranges = input[0].Split(',').Select(RepeatedDigits.Parse).ToArray();

   
    public long Part1()
    {
        return ranges.SelectMany(r => r).Sum();
    }
    public long Part2() => -1;
}

public class AoC202502Tests
{
    private readonly AoC202502 sut;
    public AoC202502Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202502(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(1227775554, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(4174379265, sut.Part2());
    }
  
}