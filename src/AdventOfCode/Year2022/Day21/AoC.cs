using AdventOfCode.Common;

namespace AdventOfCode.Year2022.Day21;
public class AoC202221
{
    static readonly bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    static readonly Regex operationRegex = new Regex(@"(?<name>[a-z]+): (?<left>[a-z]+) (?<operation>.) (?<right>[a-z]+)", RegexOptions.Compiled);
    static readonly Regex numberRegex = new Regex(@"(?<name>[a-z]+): (?<number>[\d]+)", RegexOptions.Compiled);
    static IReadOnlyDictionary<string, Operation> instructions = (
            from line in usesample ? sample : input
            select operationRegex.As<MathOperation>(line) as Operation ?? numberRegex.As<YellNumber>(line) as Operation
            ).ToDictionary(o => o.name);

    static IReadOnlyDictionary<string, MathOperation> parents = (
        from i in instructions
        where i.Value is MathOperation m
        let m = (MathOperation) i.Value
        from child in (m.left, m.right).AsEnumerable()
        let parent = m 
        select (child, parent)
        ).ToDictionary(x => x.child, x => x.parent);

    public long Part1() => GetValue("root", instructions);
    public long Part2() 
    {

        var queue = new Queue<string>();

        queue.Enqueue("humn");

        var newinstructions = new Dictionary<string, Operation>();
        while (queue.Any())
        {
            var name = queue.Dequeue();

            Operation op = instructions[name] switch
            {
                YellNumber n when name is not "humn" => n,
                _ => Transform(name)
            };

            newinstructions[op.name] = op;

            var children = op switch
            {
                MathOperation m => (m.left, m.right).AsEnumerable(),
                _ => Empty<string>()
            };
            
            foreach (var c in children)
            {
                queue.Enqueue(c);
            }
        }


        foreach (var item in newinstructions)
        {
            Console.WriteLine(item);
        }

        //return GetValue2("humn", instructions);
        return 0;
    }


    long GetValue2(string name, IReadOnlyDictionary<string, Operation> instructions)
    {
        var operation = name switch
        {
            "humn" => Transform(name),
            _ => instructions[name]
        };
        Console.WriteLine($"GetValue2: {operation}");
        return operation switch
        {
            YellNumber n => n.number,
            _ => Transform(operation.name) switch
            {
                MathOperation { operation: '+' } m => GetValue2(m.left, instructions) + GetValue2(m.right, instructions),
                MathOperation { operation: '-' } m => GetValue2(m.left, instructions) - GetValue2(m.right, instructions),
                MathOperation { operation: '*' } m => GetValue2(m.left, instructions) * GetValue2(m.right, instructions),
                MathOperation { operation: '/' } m => GetValue2(m.left, instructions) / GetValue2(m.right, instructions),
                _ => throw new Exception()
            }
        };

    }

    MathOperation Transform(string leaf)
    {
        var transformed = parents[leaf] switch
        {
            MathOperation { operation: '+' } p when p.left == leaf => new MathOperation(p.left, p.name, '-', p.right),
            MathOperation { operation: '+' } p when p.right == leaf => new MathOperation(p.right, p.name, '-', p.left),
            MathOperation { operation: '-' } p when p.left == leaf => new MathOperation(p.left, p.name, '+', p.right),
            MathOperation { operation: '-' } p when p.right == leaf => new MathOperation(p.right, p.left, '-', p.name),
            MathOperation { operation: '*' } p when p.left == leaf => new MathOperation(p.left, p.name, '/', p.right),
            MathOperation { operation: '*' } p when p.right == leaf => new MathOperation(p.right, p.name, '/', p.left),
            MathOperation { operation: '/' } p when p.left == leaf => new MathOperation(p.left, p.name, '*', p.right),
            MathOperation { operation: '/' } p when p.right == leaf => new MathOperation(p.right, p.left, '/', p.name),
            _ => throw new Exception()
        };

        Console.WriteLine($"{leaf}: {instructions[leaf]} -> {transformed}");

        return transformed;
    }

    long GetValue(string operation, IReadOnlyDictionary<string, Operation> instructions) => instructions[operation] switch
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


interface Operation
{
    string name { get; }

}

record struct YellNumber(string name, long number) : Operation;
record struct MathOperation(string name, string left, char operation, string right) : Operation;
