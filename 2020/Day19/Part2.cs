using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Part2.AoC;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
namespace Part2;
public static class Runner
{
    public static void Run()
    {
        var (rules, messages) = ReadFile("input.txt");
        Console.WriteLine(messages.Count<string>(rules.ToRegex().IsMatch));
    }
}

record Rule(int Number);
record SingleCharacter(int Number, char Value) : Rule(Number);
record RecursiveRule(int Number, ImmutableArray<ImmutableArray<int>> RuleNumberLists) : Rule(Number);

static class AoC
{
    internal static (ImmutableDictionary<int, Rule>, string[]) ReadFile(string fileName)
    {
        var enumerator = File.ReadLines(fileName).GetEnumerator();
        return (ReadRules(enumerator).ToImmutableDictionary(r => r.Number), ReadLines(enumerator).ToArray());
    }

    internal static IEnumerable<Rule> ReadRules(IEnumerator<string> enumerator)
        => from line in ReadLines(enumerator).TakeWhile(s=>!string.IsNullOrEmpty(s))
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

    // rule 8 becomes 42 | 42 8, is essentially 42 'recursive'
    // rule 11 becomes 42 31 | 42 11 31
    // for explanation, see:
    //  https://docs.microsoft.com/en-us/dotnet/standard/base-types/grouping-constructs-in-regular-expressions#balancing-group-definitions
    //  https://www.codeproject.com/articles/21183/in-depth-with-net-regex-balanced-grouping
    static string ToRegex(int n, ImmutableDictionary<int, Rule> rules)
        => rules[n] switch
        {
            SingleCharacter l => l.Value.ToString(),
            RecursiveRule r => r.Number switch
            {
                8 => $"(?:{ToRegex(42, rules)})+",
                11 => $"(?<DEPTH>{ToRegex(42, rules)})+(?<-DEPTH>{ToRegex(31,rules)})+(?(DEPTH)(?!))", 
                _ => $"({string.Join("|", from numbers in r.RuleNumberLists select string.Join("", from ruleNumber in numbers select ToRegex(ruleNumber, rules)))})"
            },
            _ => throw new()
        };

}

public class Tests
{
    [Fact]
    public void ReadExample()
    {
        var (rules, messages) = ReadFile("example2.txt");
        var regex = rules.ToRegex();
        Assert.Equal(12, messages.Count(regex.IsMatch));
    }
}