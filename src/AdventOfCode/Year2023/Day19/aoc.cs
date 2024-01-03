using Microsoft.CodeAnalysis;

namespace AdventOfCode.Year2023.Day19;
public class AoC202319
{
    public AoC202319() : this(Read.InputLines(), Console.Out) { }
    readonly TextWriter writer;
    readonly string[] input;
    readonly ImmutableDictionary<string, Workflow> workflows;
    readonly ImmutableArray<Part> parts;
    internal IReadOnlyDictionary<string, Workflow> Workflows => workflows;
    internal IEnumerable<Part> Parts => parts;
    public AoC202319(string[] input, TextWriter writer)
    {
        this.input = input;
        workflows = input.TakeWhile(s => !string.IsNullOrEmpty(s)).Select(Workflow.Parse).ToImmutableDictionary(w => w.name);
        parts = input.SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(1).Select(s => Regexes.PartRegex().As<Part>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public object Part1() => (from item in parts
                              let workflow = workflows["in"]
                              where IsAccepted(item)
                              select item.Value).Sum();
    public long Part2()
    {
        return SolveRecursive(new(), "in", workflows);
    }

    long SolveRecursive(XMAS xmas, string current, IReadOnlyDictionary<string, Workflow> workflows)
    {
        if (current == "A")
        {
            return xmas.Value;
        }
        if (current == "R")
        {
            return 0L;
        }
        long result = 0L;
        Workflow w = workflows[current];
        foreach (var rule in w.rules)
        {
            (var accepted, xmas) = xmas.Apply(rule);
            result += SolveRecursive(accepted, rule.next, workflows);
        }
        result += SolveRecursive(xmas, w.fallback, workflows);
        return result;
    }

    private bool IsAccepted(Part parts)
    {
        var wf = "in";
        while (wf != "A" && wf != "R")
        {
            var workflow = workflows[wf];
            wf = workflow.Process(parts);
        }
        return wf == "A";
    }
}

readonly record struct Workflow(string name, Rule[] rules, string fallback)
{
    public static Workflow Parse(string s)
    {
        var match = Regexes.WorkflowRegex().Match(s);
        if (!match.Success) throw new InvalidOperationException($"Regex {Regexes.WorkflowRegex()} does not match '{s}'");
        var name = match.Groups["name"].Value;
        var rules = match.Groups["rules"].Value.Split(",");
        var fallback = rules.Last();
        return new(name, rules[0..^1].Select(Rule.Parse).ToArray(), rules[^1]);
    }
    public override string ToString() => $"{name}{{{string.Join(",", rules)}}}";

    internal string Process(Part item)
    {
        foreach (var rule in rules)
        {
            if (rule.Eval(item, out var next))
            {
                return next;
            }
        }
        return fallback;
    }

}
readonly record struct Rule(string variable, char? @operator, int value, string next)
{
    public static Rule Parse(string s) => Regexes.RuleRegex().As<Rule>(s);
    public override string ToString() => $"{variable}{@operator}{value}:{next}";
    public bool Eval(Part item, out string next)
    {
        var value = variable switch
        {
            "a" => item.a,
            "m" => item.m,
            "x" => item.x,
            "s" => item.s,
        };
        next = this.next;
        return IsValid(value, this.value);
    }
    private bool IsValid(int left, int right) => @operator switch
    {
        '>' => left > right,
        '<' => left < right,
    };
}
readonly record struct Part(int x, int m, int a, int s)
{
    public int Value => x + m + a + s;
}
record struct XMAS(Range x, Range m, Range a, Range s)
{
    public XMAS() : this(new(), new(), new(), new()) { }
    public long Value => x.Value * m.Value * a.Value * s.Value;
    public (XMAS left, XMAS right) Apply(Rule rule) => rule.variable switch
    {
        "x" => (this with { x = x.Left(rule) }, this with { x = x.Right(rule) }),
        "m" => (this with { m = m.Left(rule) }, this with { m = m.Right(rule) }),
        "a" => (this with { a = a.Left(rule) }, this with { a = a.Right(rule) }),
        "s" => (this with { s = s.Left(rule) }, this with { s = s.Right(rule) })
    };
}

record struct Range(int Start, int End)
{
    public Range() : this(1, 4001) { }
    public static implicit operator Range(System.Range r) => new(r.Start.Value, r.End.Value);
    public long Value => End - Start;
    public override string ToString() => $"[{Start};{End}[";
    public Range Left(Rule r) => r.@operator switch
    {
        '<' => Start..r.value,
        '>' => (r.value + 1)..End
    };
    public Range Right(Rule r) => r.@operator switch
    {
        '<' => r.value..End,
        '>' => Start..(r.value + 1)
    };
}


static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>[^{]+)\{(?<rules>[^}]+)\}$")]
    public static partial Regex WorkflowRegex();

    [GeneratedRegex(@"^(?<variable>[xmas])(?<operator>[<>])(?<value>\d+):(?<next>[\w]+)$")]
    public static partial Regex RuleRegex();

    [GeneratedRegex(@"^\{x=(?<x>[\d]+),m=(?<m>[\d]+),a=(?<a>[\d]+),s=(?<s>[\d]+)\}$")]
    public static partial Regex PartRegex();
}

public class AoC202319Tests
{
    private readonly AoC202319 sut;
    public AoC202319Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202319(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(11, sut.Workflows.Count());
        Assert.Equal(5, sut.Parts.Count());
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(19114, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(167409079868000, sut.Part2());
    }

    [Theory]
    [InlineData(1, 10, 9)]
    [InlineData(1, 4001, 4000)]
    [InlineData(10, 19, 9)]
    public void RangeValueTest(int start, int end, int value)
    {
        var range = new Range(start, end);
        Assert.Equal(value, range.Value);
    }

}

