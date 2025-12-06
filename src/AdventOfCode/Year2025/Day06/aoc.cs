namespace AdventOfCode.Year2025.Day06;

public class AoC202506(string[] input)
{
    public AoC202506() : this(Read.InputLines()) { }

    public long Part1()
    {
        ReadOnlySpan<char> operations = input[^1];

        long total = 0;
        List<long> numbers = [];

        for (var column = 0; column < operations.Length;)
        {
            var next = column + 1;
            while (next < operations.Length && operations[next] == ' ') next++;

            var operation = operations[column];

            for (int i = 0; i < input.Length - 1; i++)
            {
                ReadOnlySpan<char> line = input[i];
                numbers.Add(long.Parse(line[column..next]));
            }
            var result = operation switch
            {
                '+' => numbers.Sum(),
                '*' => numbers.Aggregate(1L, (a, b) => a * b),
                _ => throw new InvalidOperationException()
            };

            numbers.Clear();
            total += result;
            column = next;
        }
        return total;

    }

    public long Part2() 
    {
        ReadOnlySpan<char> operations = input[^1];

        long total = 0;
        List<long> numbers = [];

        for (var column = 0; column < operations.Length;)
        {
            var next = column + 1;
            while (next < operations.Length && operations[next] == ' ') 
                next++;
            if (next == operations.Length) next++; // there is no space at the end

            var operation = operations[column];

            for (int i = next - 2; i >= column; i--)
            {
                var number = 0L;
                var multiplier = 1;
                for (int j = input.Length - 2; j >= 0; j--)
                {
                    var digit = input[j][i];
                    if (digit == ' ') continue;
                    number += (digit - '0') * multiplier;
                    multiplier *= 10;
                }
                numbers.Add(number);
            }

            var result = operation switch
            {
                '+' => numbers.Sum(),
                '*' => numbers.Aggregate(1L, (a, b) => a * b),
                _ => throw new InvalidOperationException()
            };

            numbers.Clear();
            total += result;
            column = next;
        }

        return total;
      
    }
}

public class AoC202506Tests
{
    private readonly AoC202506 sut;
    public AoC202506Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202506(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(4277556, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(3263827, sut.Part2());
    }
}