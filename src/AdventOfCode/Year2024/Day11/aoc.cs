namespace AdventOfCode.Year2024.Day11;

public class AoC202411(string input)
{
    public AoC202411() : this(Read.InputLines().First()) { }

    ImmutableList<long> list = ReadInput(input);
    static ImmutableList<long> ReadInput(string input)
    {
        var span = input.AsSpan();
        var ll = new List<long>();
        foreach (var s in span.Split(' '))
        {
            ll.Add(long.Parse(span[s]));
        }
        return [.. ll];
    }

    public long Part1() => Solve(25);
    public long Part2() => Solve(75);

    private long Solve(int iterations)
    {
        var current = list.ToDictionary(x => x, _ => 1L);

        for (int i = 0; i < iterations; i++)
        {
            var next = new Dictionary<long, long>(current.Count);

            foreach (var (k, v) in current)
            {
                if (k == 0)
                {
                    next[1] = next.GetValueOrDefault(1) + v;
                }
                else
                {
                    var digits = GetDigits(k);
                    if (digits % 2 == 0)
                    {
                        var factor = (long)Pow(10, digits / 2);
                        var (first, second) = (k/factor,k % factor);
                        next[first] = next.GetValueOrDefault(first) + v;
                        next[second] = next.GetValueOrDefault(second) + v;
                    }
                    else
                    {
                        next[k * 2024] = next.GetValueOrDefault(k * 2024) + v;
                    }
                }
            }
            current = next;
        }
        return current.Values.Sum();
    }


    private int GetDigits(long n)
    {
        int count = 0;
        while (n != 0)
        {
            n /= 10;
            ++count;
        }
        return count;
    }
}


public class AoC202411Tests
{
    private readonly AoC202411 sut;
    public AoC202411Tests(ITestOutputHelper output)
    {
        var input = Read.SampleText();
        sut = new AoC202411(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(55312, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(65601038650482, sut.Part2());
    }

    [Fact]
    public void TestGetDigits()
    {
        Assert.Equal(2, GetDigits(10));
        Assert.Equal(3, GetDigits(123));
        Assert.Equal(1, GetDigits(1));
        Assert.Equal(5, GetDigits(10000));
        Assert.Equal(4, GetDigits(9999));
    }

    private int GetDigits(long n)
    {
        int count = 0;
        while (n != 0)
        {
            n /= 10;
            ++count;
        }
        return count;
    }

}

