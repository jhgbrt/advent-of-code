﻿using static AdventOfCode.Year2020.Day19.Part1.AoC;
namespace AdventOfCode.Year2020.Day19.Part1;

public static class Runner
{
    public static object Run()
    {
        var (rules, messages) = ReadFile(Read.InputLines());
        return messages.Count<string>(rules.ToRegex().IsMatch);
    }
}

record Rule(int Number);
record SingleCharacter(int Number, char Value) : Rule(Number);
record RecursiveRule(int Number, ImmutableArray<ImmutableArray<int>> RuleNumberLists) : Rule(Number);

static class AoC
{
    internal static (ImmutableDictionary<int, Rule>, string[]) ReadFile(IEnumerable<string> input)
    {
        var enumerator = input.GetEnumerator();
        return (ReadRules(enumerator).ToImmutableDictionary(r => r.Number), ReadLines(enumerator).ToArray());
    }

    internal static IEnumerable<Rule> ReadRules(IEnumerator<string> enumerator)
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

    public static Regex ToRegex(this ImmutableDictionary<int, Rule> rules) => new Regex($"^{ToRegex(0, rules)}$");

    static string ToRegex(int n, ImmutableDictionary<int, Rule> rules) => rules[n] switch
        {
            SingleCharacter l => l.Value.ToString(),
            RecursiveRule r => $"({string.Join("|", from numbers in r.RuleNumberLists select string.Join("", from ruleNumber in numbers select ToRegex(ruleNumber, rules)))})",
            _ => throw new()
        };

}
