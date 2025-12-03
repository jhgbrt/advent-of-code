namespace AdventOfCode.Year2025.Day03;

public class AoC202503(Stream input)
{
    public AoC202503() : this(Read.InputStream()) { }

    public long Part1() => ReadAllNDigitNumbers(input, 2).Sum();
    public long Part2() => ReadAllNDigitNumbers(input, 12).Sum();

    IEnumerable<long> ReadAllNDigitNumbers(Stream input, int n)
    {
        using var reader = new StreamReader(input);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return GetLargestNDigitNumber(line, n);
        }
    }

    internal static long GetLargestNDigitNumber(ReadOnlySpan<char> input, int n)
    {
        // Construct the largest possible n-digit number from the digits in the input string.
        // For each position in the result, select largest available digit that leaves
        // enough remaining digits for the subsequent positions.
        
        long result = 0; // Accumulates the resulting n-digit number
        
        int start = 0; // Starting index for the current search range in the input
        for (int i = 0; i < n; i++) // Loop for each digit position in the result
        {
            // Calculate the end index: we can search up to input.Length - (n - i) to ensure
            // there are at least (n - i - 1) digits left after this position for the remaining positions
            int end = input.Length - (n - i); 
            int max = -1; // Tracks the maximum digit found in the current search range
            for (int j = start; j <= end; j++) // Search from start to end for the largest digit
            {
                int digit = input[j] - '0'; // Convert char digit to int
                if (digit > max)
                {
                    max = digit;
                    start = j + 1;  // Start search for next digit after this one
                    if (digit == 9) // If we found a '9', we can't do better, so break early
                        break;
                }
            }
            // Append the max digit to the result
            result = result * 10 + max;
        }

        return result;
    }
}

public class AoC202503Tests
{
    private readonly AoC202503 sut;
    public AoC202503Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202503(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Theory]
    [InlineData("987654321111111", 98)]
    [InlineData("811111111111119", 89)]
    [InlineData("234234234234278", 78)]
    [InlineData("818181911112111", 92)]
    public void ExamplesPart1(string input, int expected)
    {
        // get the largest 2-digit number from the input string
        var actual = AoC202503.GetLargestNDigitNumber(input, 2);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("987654321111111", 987654321111L)]
    [InlineData("811111111111119", 811111111119L)]
    [InlineData("234234234234278", 434234234278L)]
    [InlineData("818181911112111", 888911112111L)]
    public void ExamplesPart2(string input, long expected)
    {
        // get the largest 12-digit number from the input string
        var actual = AoC202503.GetLargestNDigitNumber(input, 12);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(357L, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(3121910778619L, sut.Part2());
    }
}