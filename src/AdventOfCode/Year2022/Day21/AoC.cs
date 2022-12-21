using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day21;
public class AoC202221
{
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    static readonly Regex operationRegex = new Regex(@"(?<name>[a-z]+): (?<left>[a-z]+) (?<operation>.) (?<right>[a-z]+)", RegexOptions.Compiled);
    static readonly Regex numberRegex = new Regex(@"(?<name>[a-z]+): (?<number>[\d]+)", RegexOptions.Compiled);
    static IReadOnlyDictionary<string, Operation> instructions = (
            from line in input
            select operationRegex.As<MathOperation>(line) as Operation ?? numberRegex.As<YellNumber>(line) as Operation
            ).ToDictionary(o => o.name);

    static IReadOnlyDictionary<string, MathOperation> parents = (
        from i in instructions
        where i.Value is MathOperation m
        let m = (MathOperation) i.Value
        from child in (m.left, m.right).AsEnumerable()
        let parent = (MathOperation)instructions[child] 
        select (child, parent)
        ).ToDictionary(x => x.child, x => x.parent);

    public long Part1() => GetValue("root", instructions);
    public long Part2()
    {

        var root = (MathOperation)instructions["root"];
        var (left, right) = (root.left, root.right);

        return -1;
    }

    MathOperation Transform(string leaf, IReadOnlyDictionary<string, Operation> instructions, IDictionary<string, Operation> result)
    {
        return parents[leaf] switch
        {
            MathOperation {operation: '+' } p when p.left == leaf => new MathOperation(p.left, p.name, '-', p.right),
            MathOperation {operation: '+' } p when p.right == leaf => new MathOperation(p.right, p.name, '-', p.left),
            MathOperation { operation: '-' } p when p.left == leaf => new MathOperation(p.left, p.name, '+', p.right),
            MathOperation { operation: '-' } p when p.right == leaf => new MathOperation(p.right, p.left, '-', p.name),
            MathOperation { operation: '*' } p when p.left == leaf => new MathOperation(p.left, p.name, '/', p.right),
            MathOperation { operation: '*' } p when p.right == leaf => new MathOperation(p.right, p.name, '/', p.left),
            MathOperation { operation: '/' } p when p.left == leaf => new MathOperation(p.left, p.name, '*', p.right),
            MathOperation { operation: '/' } p when p.right == leaf => new MathOperation(p.right, p.left, '/', p.name),
            _ => throw new Exception()
        };

    }

    long GetValue(string operation, IReadOnlyDictionary<string, Operation> instructions)
    {
        return instructions[operation] switch
        {
            YellNumber n => n.number,
            MathOperation m => m.operation switch
            {
                '+' => GetValue(m.left, instructions) + GetValue(m.right, instructions),
                '-' => GetValue(m.left, instructions) - GetValue(m.right, instructions),
                '*' => GetValue(m.left, instructions) * GetValue(m.right, instructions),
                '/' => GetValue(m.left, instructions) / GetValue(m.right, instructions),
                _ => throw new Exception()
            },
            _ => throw new Exception()
        };
    }

}


interface Operation
{
    string name { get; }

}

record struct YellNumber(string name, long number) : Operation;
record struct MathOperation(string name, string left, char operation, string right) : Operation;
