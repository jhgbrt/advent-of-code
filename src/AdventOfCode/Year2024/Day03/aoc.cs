namespace AdventOfCode.Year2024.Day03;

public class AoC202403(string input)
{
    public AoC202403() : this(Read.InputText()) {}

    public int Part1() => GetResult(input, false);

    public int Part2() => GetResult(input, true);

    private static int GetResult(ReadOnlySpan<char> span, bool activeOnly)
    {
        int result = 0;
        bool shouldreturn = true;
        foreach (var m in Regexes.AoC202403Regex().EnumerateMatches(span))
        {
            var slice = span.Slice(m.Index, m.Length);
            shouldreturn = !activeOnly || slice switch
            {
                "do()" => true,
                "don't()" => false,
                _ => shouldreturn
            };

            if (shouldreturn && slice.StartsWith("mul("))
            {
                var openbracket = slice.IndexOf('(');
                var comma = slice.IndexOf(',');
                var closebracket = slice.IndexOf(')');
                var left = int.Parse(slice[(openbracket + 1)..comma]);
                var right = int.Parse(slice[(comma + 1)..closebracket]);
                result += left * right;
            }
        }
        return result;
    }

}

static partial class Regexes
{
    [GeneratedRegex(@"(mul\((?<left>\d+),(?<right>\d+)\)|do\(\)|don't\(\))")]
    public static partial Regex AoC202403Regex();
}

public class AoC202403Tests
{
    private readonly AoC202403 sut;
    public AoC202403Tests(ITestOutputHelper output)
    {
        var input = Read.SampleText();
        sut = new AoC202403(input);
    }

    [Fact]
    public void TestParsing()
    {

    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(161, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(48, sut.Part2());
    }
}