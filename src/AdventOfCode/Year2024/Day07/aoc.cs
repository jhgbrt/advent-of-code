namespace AdventOfCode.Year2024.Day07;

public class AoC202407(IEnumerable<string> input)
{
    public AoC202407() : this(Read.Input().Lines()) 
    {
    }

    readonly Equation[] equations = ParseInput(input).ToArray();

    static IEnumerable<Equation> ParseInput(IEnumerable<string> input)
    {
        foreach (ReadOnlySpan<char> line in input)
        {
            var separator = line.IndexOf(':');
            var result = long.Parse(line[..separator]);
            var numbers = line[(separator + 2)..];
            var list = new List<long>(numbers.Count(' ') + 1);
            foreach (var range in numbers.Split(" "))
            {
                list.Add(long.Parse(numbers[range]));
            }
            yield return new Equation(result, list);
        }
    }

    public long Part1() => equations.Where(e => e.IsValid(2)).Sum(e => e.target);
    public long Part2() => equations.AsParallel().Where(e => e.IsValid(3)).Sum(e => e.target);
}
enum Operation
{
    Add, Multiply, Concatenate
}

readonly record struct Equation(long target, List<long> numbers)
{
    public bool IsValid(int n)
    {
        var target = this.target;

        int combinations = (int)Pow(n, numbers.Count - 1);
        for (int i = 0; i < combinations; i++)
        {
            var result = numbers[0];
            var mask = i;

            for (int j = 1; j < numbers.Count; j++)
            {
                (var operation, mask) = ((Operation)(mask % n), mask /= n);
                result = operation switch 
                {
                    Operation.Add => result + numbers[j],
                    Operation.Multiply => result * numbers[j],
                    Operation.Concatenate => result * numbers[j] switch
                    {
                        < 10 => 10,
                        < 100 => 100,
                        < 1000 => 1000
                    } + numbers[j]
                };
                if (result > target)
                {
                    break;
                }
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
        var input = Read.SampleLines();
        sut = new AoC202407(input);
    }

    [Fact]
    public void TestParsing()
    {
    }
    
    [Fact]
    public void EquationIsValidTest()
    {
        var equation = new Equation(1344480, [7,466,85,43,7,56,424]);
        Assert.True(equation.IsValid(3));
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

