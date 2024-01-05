namespace AdventOfCode.Year2023.Day01;
public class AoC202301
{
    string[] input;
    public AoC202301(): this(Read.InputLines()) { }
    public AoC202301(string[] input)
    {
        this.input = input;
    }

    public object Part1()
    {
        var query = from line in input
                    let first = line.First(Char.IsDigit) - '0'
                    let last = line.Last(Char.IsDigit) - '0'
                    let n = first * 10 + last
                    select n;

        return query.Sum();
    }
    public object Part2() 
    {
        var list = new List<int>();
        foreach (var line in input)
        {
            var digits = ExtractDigits(line).ToArray();
            var first = digits.First();
            var last = digits.Last();
            var n = first * 10 + last;
            list.Add(n);
        }

        return list.Sum();
    }

    internal static List<int> ExtractDigits(string line)
    {
        var start = 0;
        var end = 0;
        List<int> digits = new();

        while (end < line.Length)
        {
            end++;

            for (int i = start; i < end; i++)
            {
                (var digit, bool back) = line.Substring(i, end - i) switch
                {
                    [char d] when d >= '0' && d <= '9' => (d - '0', false), 
                    ['z', 'e', 'r', 'o'] => (0, true),
                    ['o', 'n', 'e'] => (1, true),
                    ['t', 'w', 'o'] => (2, true),
                    ['t', 'h', 'r', 'e', 'e'] => (3, true),
                    ['f', 'o', 'u', 'r'] => (4, false),
                    ['f', 'i', 'v', 'e'] => (5, true),
                    ['s', 'i', 'x'] => (6, false),
                    ['s', 'e', 'v', 'e', 'n'] => (7, true),
                    ['e', 'i', 'g', 'h', 't'] => (8, true),
                    ['n', 'i', 'n', 'e'] => (9, true),
                    _ => (-1, false)
                };

                if (digit >= 0)
                {
                    digits.Add(digit);
                    if (end < line.Length && back) end--;
                    start = end;
                    break;
                }
            }
        }
        return digits;
    }
}

public class AoC202301Tests
{
    [Fact]
    public void TestPart1()
    {
        var sut = new AoC202301(Read.SampleLines(1));
        Assert.Equal(142, sut.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        var sut = new AoC202301(Read.SampleLines(1));
        Assert.Equal(281, sut.Part2());
    }

    [Theory]
    [InlineData("one2three", 1, 2, 3)]
    [InlineData("onetwothree", 1, 2, 3)]
    [InlineData("eightwothree", 8, 2, 3)]
    [InlineData("two1nine", 2, 1, 9)]
    [InlineData("abcone2threexyz", 1, 2, 3)]
    [InlineData("xtwone3four", 2, 1, 3, 4)]
    [InlineData("4nineeightseven2", 4, 9, 8, 7, 2)]
    [InlineData("zoneight234", 1, 8, 2, 3, 4)]
    [InlineData("7pqrstsixteen", 7, 6)]

    public void Test(string input, params int[] expected)
    {
        var result = AoC202301.ExtractDigits(input);
        Assert.Equal(expected, result);
    }
}