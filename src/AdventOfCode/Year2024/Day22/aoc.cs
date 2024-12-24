using System.Runtime.CompilerServices;
using Sequence = (byte, byte, byte, byte);

namespace AdventOfCode.Year2024.Day22;

public class AoC202422(string[] input)
{
    public AoC202422() : this(Read.InputLines()) {}

    long[] numbers = input.Select(long.Parse).ToArray();

    public long Part1() => numbers.Select(n => GetSecret(n, 2000)).Sum();

    private long GetSecret(long secret, int n)
    {
        for (int i = 0; i < n; i++)
        {
            secret = Next(secret);
        }
        return secret;
    }

    public int Part2()
    {
        var totals = new Dictionary<Sequence, int>();
        Span<byte> prices = new byte[2000];
        var seen = new HashSet<Sequence>();

        foreach (var n in numbers)
        {
            seen.Clear();

            var next = n;
            prices[0] = (byte)(next%10);
            for (int i = 1; i < prices.Length; i++)
            {
                next = Next(next);
                prices[i] = (byte)(next % 10);
            }

            for (int i = 4; i < prices.Length; i++)
            {
                var span = prices[(i - 4)..(i + 1)];
                var diff = (
                    (byte)(span[1] - span[0]),
                    (byte)(span[2] - span[1]),
                    (byte)(span[3] - span[2]),
                    (byte)(span[4] - span[3])
                    );

                if (seen.Add(diff))
                {
                    if (!totals.ContainsKey(diff)) totals[diff] = 0;
                    totals[diff] += prices[i];
                }
            }
        }

        return totals.Values.Max();
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
    private static long Next(long secret)
    {
        secret = ((secret <<  6) ^ secret) & (0x1000000 - 1);
        secret = ((secret >>  5) ^ secret) & (0x1000000 - 1);
        secret = ((secret << 11) ^ secret) & (0x1000000 - 1);
        return secret;
    }

}

public class AoC202422Tests(ITestOutputHelper output)
{

    [Fact]
    public void TestParsing()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202422(input);
    }

    [Fact]
    public void Test()
    {
        output.WriteLine(Math.Pow(2, 24).ToString());
        output.WriteLine(0x1000000.ToString());

        Assert.Equal(0x1000000, (int)Math.Pow(2, 24));
        Assert.Equal(0x0800000, (int)Math.Pow(2, 23));
        output.WriteLine($"{(int)Math.Pow(2, 23)}");
        Assert.Equal(123123887681726 % 0x1000000, 123123887681726 & (0x1000000 - 1));

    }

    [Fact]
    public void Test2()
    {
        var secret = 123;
        secret = ((secret << 6) ^ secret) & (0x1000000 - 1);
        output.WriteLine(secret.ToString());
        secret = ((secret >> 5) ^ secret) & (0x1000000 - 1);
        output.WriteLine(secret.ToString());
        secret = ((secret << 11) ^ secret) & (0x1000000 - 1);
        output.WriteLine(secret.ToString());
    }

    [Fact]
    public void TestPart1()
    {
        var input = Read.SampleLines(1);
        var sut = new AoC202422(input);
        Assert.Equal(37327623, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        var input = Read.SampleLines(2);
        var sut = new AoC202422(input);
        Assert.Equal(23, sut.Part2());
    }
}