namespace AdventOfCode.Year2020.Day19;

public class AoC202019
{
    string[] input = Read.InputLines();
    public object Part1()
    {
        var (rules, messages) = Parse(input);
        return messages.Count(rules.ToRegex1().IsMatch);
    }

    public object Part2()
    {
        var (rules, messages) = Parse(Read.InputLines());
        return messages.Count<string>(rules.ToRegex2().IsMatch);
    }

    static (ImmutableDictionary<int, Rule>, string[]) Parse(IEnumerable<string> input)
    {
        var enumerator = input.GetEnumerator();
        return (ReadRules(enumerator).ToImmutableDictionary(r => r.Number), ReadLines(enumerator).ToArray());
    }

    static IEnumerable<Rule> ReadRules(IEnumerator<string> enumerator)
        => from line in ReadLines(enumerator).TakeWhile(s => !string.IsNullOrEmpty(s))
           let parts = line.Split(": ")
           let ruleNumber = int.Parse(parts[0])
           select parts[1][0] switch
           {
               '"' => new SingleCharacter(ruleNumber, parts[1][1]) as Rule,
               _ => new RecursiveRule(ruleNumber, (from p in parts[1].Split(" | ")
                                                   let numbers = (from i in p.Split(' ')
                                                                  select int.Parse(i)).ToImmutableArray()
                                                   select numbers).ToImmutableArray())
           };

    static IEnumerable<string> ReadLines(IEnumerator<string> enumerator)
    {
        while (enumerator.MoveNext()) yield return enumerator.Current;
    }
}

record Rule(int Number);
record SingleCharacter(int Number, char Value) : Rule(Number);
record RecursiveRule(int Number, ImmutableArray<ImmutableArray<int>> RuleNumberLists) : Rule(Number);

static class Ex
{

    public static Regex ToRegex1(this ImmutableDictionary<int, Rule> rules) => new Regex($"^{ToRegex1(0, rules)}$");

    static string ToRegex1(int n, ImmutableDictionary<int, Rule> rules) => rules[n] switch
    {
        SingleCharacter l => l.Value.ToString(),
        RecursiveRule r => $"({string.Join("|", from numbers in r.RuleNumberLists select string.Join("", from ruleNumber in numbers select ToRegex1(ruleNumber, rules)))})",
        _ => throw new()
    };

    public static Regex ToRegex2(this ImmutableDictionary<int, Rule> rules) => new Regex($"^{ToRegex2(0, rules)}$");

    // rule 8 becomes 42 | 42 8, is essentially 42 'recursive'
    // rule 11 becomes 42 31 | 42 11 31
    // for explanation, see:
    //  https://docs.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#balancing-group-definitions
    //  https://www.codeproject.com/articles/21183/in-depth-with-net-regex-balanced-grouping
    static string ToRegex2(int n, ImmutableDictionary<int, Rule> rules)
        => rules[n] switch
        {
            SingleCharacter l => l.Value.ToString(),
            RecursiveRule r => r.Number switch
            {
                8 => $"(?:{ToRegex2(42, rules)})+",
                11 => $"(?<DEPTH>{ToRegex2(42, rules)})+(?<-DEPTH>{ToRegex2(31, rules)})+(?(DEPTH)(?!))",
                _ => $"({string.Join("|", from numbers in r.RuleNumberLists select string.Join("", from ruleNumber in numbers select ToRegex2(ruleNumber, rules)))})"
            },
            _ => throw new()
        };

}
