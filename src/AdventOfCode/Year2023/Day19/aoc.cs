namespace AdventOfCode.Year2023.Day19;
public class AoC202319
{
    public AoC202319():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableDictionary<string, Workflow> workflows;
    readonly ImmutableArray<Part> parts;
    internal IReadOnlyDictionary<string, Workflow> Workflows => workflows;
    internal IEnumerable<Part> Parts => parts;
    public AoC202319(string[] input, TextWriter writer)
    {
        workflows = input.TakeWhile(s => !string.IsNullOrEmpty(s)).Select(Workflow.Parse).ToImmutableDictionary(w => w.name);
        parts = input.SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(1).Select(s => Regexes.PartRegex().As<Part>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public object Part1() => (from item in parts
                              let workflow = workflows["in"]
                              where IsAccepted(item)
                              select item.Value).Sum();
    public object Part2() => "";
    
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


readonly record struct Workflow(string name, Rule[] rules)
{
    public static Workflow Parse(string s)
    {
        var match = Regexes.WorkflowRegex().Match(s);
        if (!match.Success) throw new InvalidOperationException($"Regex {Regexes.WorkflowRegex()} does not match '{s}'");
        var name = match.Groups["name"].Value;
        var rules = match.Groups["rules"].Value.Split(",").Select(Rule.Parse);
        return new(name, rules.ToArray());
    }
    public override string ToString()
    {
        return $"{name}{{{string.Join(",", rules)}}}";
    }

    internal string Process(Part item)
    {
        foreach (var rule in rules)
        {
            if (rule.TryMatch(item, out var next))
            {
                return next;
            }
        }
        return "";
    }
}



readonly record struct Rule(string variable, char? @operator, int? value, string next)
{
    public static Rule Parse(string s)
    {
        var match = Regexes.RuleRegex().Match(s);
        if (!match.Success) throw new InvalidOperationException($"Regex {Regexes.RuleRegex()} does not match '{s}'");
        var variable = match.Groups["variable"].Value;
        var @operator = match.Groups["operator"].Length > 0
            ? match.Groups["operator"].Value[0]
            : (char?)null; 
        var value = !string.IsNullOrEmpty(match.Groups["value"].Value) 
            ? int.Parse(match.Groups["value"].Value)
            : (int?)null;
        var next = match.Groups["next"].Value;
        return new(variable, @operator, value, next);
    }
    public override string ToString()
    {
        return value.HasValue ? $"{variable}{@operator}{value}:{next}" : variable;
    }
    public bool TryMatch(Part item, out string next)
    {
        if (!this.value.HasValue)
        {
            next = this.variable;
            return true;
        }

        var value = this.variable switch
        {
            "a" => item.a,
            "m" => item.m,
            "x" => item.x,
            "s" => item.s,
        };
        Func<(int left, int right), bool> f = @operator switch
        {
            '>' => p => p.left > p.right ,
            '<' => p => p.left < p.right ,
        };
        next = this.next;
        return f((value, this.value.Value));
    }
}
readonly record struct Part(int x, int m, int a, int s)
{
    public int Value => x + m + a + s;
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>[^{]+)\{(?<rules>[^}]+)\}$")]
    public static partial Regex WorkflowRegex();

    [GeneratedRegex(@"^(?<variable>[\w]+)((?<operator>[<>])(?<value>\d+):(?<next>[\w]+))?$")]
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
        //Assert.Equal("foo", sut.Items.First().next);
        //Assert.Equal(1, sut.Items.First().n);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(19114, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }
}