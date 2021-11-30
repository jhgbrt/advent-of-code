namespace AdventOfCode.Year2015.Day23;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));
    static ImmutableList<Instruction> instructions = (
        from line in input
        let split = line.Split(' ')
        let name = split[0]
        let instruction = name switch
        {
            "hlf" => new Hlf(split[1].Single()) as Instruction,
            "tpl" => new Tpl(split[1].Single()),
            "inc" => new Inc(split[1].Single()),
            "jmp" => new Jmp(int.Parse(split[1])),
            "jie" => new Jie(split[1].First(), int.Parse(split[2])),
            "jio" => new Jio(split[1].First(), int.Parse(split[2])),
            _ => throw new Exception()
        }
        select instruction).ToImmutableList();


    public override object Part1() => Run(instructions, 0);
    public override object Part2() => Run(instructions, 1);

    static int Run(IReadOnlyCollection<Instruction> instructions, int a)
    {
        var memory = new Dictionary<char, int>()
        {
            ['a'] = a,
            ['b'] = 0,
        };

        var index = 0;
        while (index >= 0 && index < instructions.Count)
        {
            var i = instructions.ElementAt(index);
            index = i.Apply(index, memory);
        }
        return memory['b'];

    }
}
readonly record struct Hlf(char register) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory)
    {
        memory[register] /= 2;
        return index + 1;
    }
}
readonly record struct Tpl(char register) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory)
    {
        memory[register] *= 3;
        return index + 1;
    }
}

readonly record struct Inc(char register) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory)
    {
        memory[register] += 1;
        return index + 1;
    }
}
readonly record struct Jmp(int offset) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory) => index + offset;
}

readonly record struct Jie(char register, int offset) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory) => index + (memory[register] % 2 == 0 ? offset : 1);
}

readonly record struct Jio(char register, int offset) : Instruction
{
    public int Apply(int index, IDictionary<char, int> memory) => index + (memory[register] == 1 ? offset : 1);
}

interface Instruction
{
    int Apply(int index, IDictionary<char, int> memory);
}
