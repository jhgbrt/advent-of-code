using Operator = System.Func<long, long, long>;
namespace AdventOfCode.Year2024.Day07;

public class AoC202407(Stream input)
{
    public AoC202407() : this(Read.InputStream()) {}

    readonly Equation[] equations = ParseInput(input).ToArray();
    static readonly Operator[] operators1 = [Add, Multiply];
    static readonly Operator[] operators2 = [Add, Multiply, Concatenate];
    static IEnumerable<Equation> ParseInput(Stream input)
    {
        var sr = new StreamReader(input);
        while (sr.ReadLine() is string line)
        {
            var span = line.AsSpan();
            var separator = span.IndexOf(':');
            var result = long.Parse(span[..separator]);
            var numbers = span[(separator + 2)..];
            var list = new List<long>(numbers.Count(' ') + 1);
            foreach (var range in Regex.EnumerateSplits(span[(separator + 2)..], " "))
            {
                list.Add(long.Parse(numbers[range]));
            }
            yield return new Equation(result, list);
        }
    }

    public long Part1() => equations.Where(e => e.IsValid(operators1)).Sum(e => e.target);
    public long Part2() => equations.Where(e => e.IsValid(operators2)).Sum(e => e.target);

    static long Add(long left, long right) => left + right;
    static long Multiply(long left, long right) => left * right;
    static long Concatenate(long left, long right)
    {
        long factor = 1;
        while (factor <= right)
        {
            factor *= 10;
        }
        return left * factor + right;
    }

}

readonly record struct Equation(long target, List<long> numbers)
{
    public bool IsValid(Operator[] operators)
    {

        int n = operators.Length;
        int combinations = (int)Pow(n, numbers.Count - 1);

        for (int i = 0; i < combinations; i++)
        {
            var result = numbers[0];
            var mask = i;

            for (int j = 1; j < numbers.Count; j++)
            {
                int index = mask % n;
                mask /= n;
                result = operators[index](result, numbers[j]);
            }

            if (result == target)
            {
                return true;
            }
        }

        return false;
    }
}


public class AoC202407Tests
{
    private readonly AoC202407 sut;
    public AoC202407Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202407(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(3749, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(11387, sut.Part2());
    }
}

