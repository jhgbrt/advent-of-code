namespace AdventOfCode.Year2024.Day03;

public class AoC202403(string input)
{
    public AoC202403() : this(Read.InputText()) {}

    public int Part1() => tokens.OfType<@mul>().Select(m => m.result).Sum();
    public int Part2() => GetActiveTokens(tokens).Select(m => m.result).Sum();

    readonly List<Instruction> tokens = Parse(input);

    private static List<Instruction> Parse(ReadOnlySpan<char> span)
    {
        List<Instruction> tokens = [];
        foreach (var m in Regexes.AoC202403Regex().EnumerateMatches(span))
        {
            var slice = span.Slice(m.Index, m.Length);
            tokens.Add(slice switch
            {
                "do()" => new @do(),
                "don't()" => new @dont(),
                _ => @mul.Parse(slice)
            });
        }
        return tokens;
    }

    private static IEnumerable<@mul> GetActiveTokens(IList<Instruction> tokens)
    {
        bool active = true;

        foreach (var token in tokens)
        {
            active = token switch
            {
                @do _ => true,
                @dont _ => false,
                _ => active
            };
            if (active && token is mul m)
            {
                yield return m;
            }
        }
    }
}

interface Instruction { }
readonly struct @mul(int l, int r) : Instruction 
{
    public int result => l * r;
    public static Instruction Parse(ReadOnlySpan<char> slice)
    {
        var mul = slice[4..^1];
        var separator = mul.IndexOf(',');
        var l = int.Parse(mul[0..separator]);
        var r = int.Parse(mul[(separator+1)..]);
        return new mul(l, r);
    }
} 
readonly struct @do : Instruction;
readonly struct @dont : Instruction;

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