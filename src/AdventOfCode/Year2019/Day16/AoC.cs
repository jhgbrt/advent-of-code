namespace AdventOfCode.Year2019.Day16;
public class AoC201916
{
    public AoC201916():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<byte> digits;

    public AoC201916(string[] input, TextWriter writer)
    {
        this.writer = writer;
        digits = input[0].Select(c => (byte)(c - '0')).ToImmutableArray();
    }

    public string Part1()
    {
        var result = digits.ToArray();
        var cache = new byte[digits.Length];
        for (int i = 0; i < 100; i++) 
        { 
            (result, cache) = Solve(result, cache); 
        }
        return string.Join("", result.Take(8));
    }

    public (byte[] result, byte[] cache) Solve(byte[] digits, byte[] cache)
    {
        for (int i = 0; i < digits.Length; i++)
        {
            cache[i] = (byte)(Abs(digits.Zip(Pattern(i + 1).Skip(1).Take(digits.Length), (a, b) => a * b).Sum()) % 10);
        }
        return (cache, digits);
    }

    static readonly int[] basepattern = [0, 1, 0, -1];
    IEnumerable<int> Pattern(int repeat)
    {
        while (true)
            for (int i = 0; i < 4; i++)
            {
                for (var j = 0; j < repeat; j++)
                {
                    yield return basepattern[i];
                }
            }
    }

    public string Part2() => Solve(10000, int.Parse(string.Join("", digits.Take(7))));

    private string Solve(int repeats, int offset)
    {
        var result = new byte[digits.Length * repeats];
        var buffer = new byte[result.Length];

        for (int i = 0; i < repeats; i++)
        {
            digits.CopyTo(0, result, digits.Length * i, digits.Length);
        }

        for (int i = 0; i < 100; i++)
        {
            (result, buffer) = Solve(result, buffer, offset);
        }
        return string.Join("", result.Skip(offset).Take(8));
    }

    private static (byte[] result, byte[] cache) Solve(byte[] input, byte[] buffer, int from = 0)
    {
        var sum = 0;

        for (int k = from; k < input.Length; k++)
        {
            sum += input[k];
        }

        for (int i = from; i < input.Length; i++)
        {
            buffer[i] = (byte)(sum % 10);
            sum -= input[i];
        }

        return (buffer, input);
    }


}

public class AoC201916Tests
{
    private readonly ITestOutputHelper output;
    public AoC201916Tests(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Theory]
    [InlineData("80871224585914546619083218645595", 24176176)]
    [InlineData("19617804207202209144916044189917", 73745418)]
    [InlineData("69317163492948606335995924319873", 52432133)]
    public void TestPart1(string input, int expected)
    {
        var sut = new AoC201916([input], new TestWriter(output));
        Assert.Equal(expected.ToString(), sut.Part1());
    }

    [Theory]
    [InlineData("03036732577212944063491565474664", 84462026)]
    [InlineData("02935109699940807407585447034323", 78725270)]
    [InlineData("03081770884921959731165446850517", 53553731)]
    public void TestPart2(string input, int expected)
    {
        var sut = new AoC201916([input], new TestWriter(output));
        Assert.Equal(expected.ToString(), sut.Part2());
    }

   
}