using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day21;
public class AoC202221
{
    static string[] input = Read.InputLines();

    static readonly Regex calculationRegex = new Regex(@"(?<name>[a-z]+): (?<l>[a-z]+) (?<operation>.) (?<r>[a-z]+)", RegexOptions.Compiled);
    static readonly Regex numberRegex = new Regex(@"(?<name>[a-z]+): (?<number>[\d]+)", RegexOptions.Compiled);
    static readonly ImmutableDictionary<string, Instruction> instructions = (
            from line in input
            select calculationRegex.IsMatch(line)
                ? calculationRegex.As<Calculation>(line) as Instruction
                : numberRegex.As<Number>(line)
            ).ToImmutableDictionary(o => o.name);

    static readonly IReadOnlyDictionary<string, Calculation> parents = (
        from i in instructions
        where i.Value is Calculation m
        let m = (Calculation) i.Value
        from child in (m.l, m.r).AsEnumerable()
        let parent = m
        select (child, parent)
        ).ToDictionary(x => x.child, x => x.parent);

    public long Part1() => GetValue("root", instructions);
    public long Part2() => GetValue("humn", TransformInstructions(instructions).ToImmutableDictionary(x => x.name));

    static IEnumerable<Instruction> TransformInstructions(IReadOnlyDictionary<string, Instruction> instructions)
    {
        var name = "humn";
        while (true)
        {
            var parent = parents[name];
            var sibling = parent.Other(name);

            if (parent is { name: "root" })
            {
                yield return new Number(name, GetValue(sibling, instructions));
                break;
            }

            var transformed = Transform(name, parent);
            var number = new Number(sibling, GetValue(sibling, instructions));
            yield return transformed;
            yield return number;

            name = transformed.Other(sibling);
        }
    }

    static Calculation Transform(string child, Calculation parent) => parent.operation switch
    {
        '+' when parent.l == child => new(parent.l, parent.name, '-', parent.r),
        '+' when parent.r == child => new(parent.r, parent.name, '-', parent.l),
        '-' when parent.l == child => new(parent.l, parent.name, '+', parent.r),
        '-' when parent.r == child => new(parent.r, parent.l, '-', parent.name),
        '*' when parent.l == child => new(parent.l, parent.name, '/', parent.r),
        '*' when parent.r == child => new(parent.r, parent.name, '/', parent.l),
        '/' when parent.l == child => new(parent.l, parent.name, '*', parent.r),
        '/' when parent.r == child => new(parent.r, parent.l, '/', parent.name),
        _ => throw new Exception()
    };

    static long GetValue(string operation, IReadOnlyDictionary<string, Instruction> instructions) => instructions[operation] switch
    {
        Number n => n.number,
        Calculation m => m.operation switch
        {
            '+' => GetValue(m.l, instructions) + GetValue(m.r, instructions),
            '-' => GetValue(m.l, instructions) - GetValue(m.r, instructions),
            '*' => GetValue(m.l, instructions) * GetValue(m.r, instructions),
            '/' => GetValue(m.l, instructions) / GetValue(m.r, instructions),
            _ => throw new Exception()
        },
        _ => throw new Exception()
    };
}

interface Instruction
{
    string name { get; }
}
record struct Number(string name, long number) : Instruction
{
    public override string ToString() => $"{name}: {number}";
}
record struct Calculation(string name, string l, char operation, string r) : Instruction
{
    public override string ToString() => $"{name}: {l} {operation} {r}";
    public string Other(string name) => name == l ? r : l;
}
