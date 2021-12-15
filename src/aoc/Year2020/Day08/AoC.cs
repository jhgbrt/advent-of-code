namespace AdventOfCode.Year2020.Day08;

public class AoC202008 
{
    static Prog program = ReadProgram();

    public object Part1() => program.Part1();
    public object Part2() => program.Part2();

    internal static Prog ReadProgram() => new Prog((
        from line in Read.InputLines()
        let instruction = line[0..3]
        let arg = int.Parse(line[3..])
        select new Instruction(instruction, arg)
    ).ToImmutableArray());

}
record Instruction(string Name, int Arg);
record Prog(ImmutableArray<Instruction> Instructions)
{
    public long Part2()
    {
        for (int i = 0; i < Instructions.Length; i++)
        {
            var fixedInstructions = Instructions.ToList();
            fixedInstructions[i] = fixedInstructions[i] switch
            {
                { Name: "jmp" } instruction => instruction with { Name = "nop" },
                { Name: "nop" } instruction => instruction with { Name = "jmp" },
                Instruction instruction => instruction
            };
            var result = Run(fixedInstructions);
            if (result.index == fixedInstructions.Count)
                return result.result;
        }
        return 0;
    }

    public long Part1() => Run(Instructions).result;

    private static (long result, int index) Run(IList<Instruction> instructions)
    {
        int accumulator = 0;
        HashSet<int> set = new();
        int i = 0;
        while (i < instructions.Count)
        {
            var instruction = instructions[i];

            if (set.Contains(i))
                break;

            set.Add(i);

            (accumulator, i) = instruction switch
            {
                { Name: "acc" } => (accumulator + instruction.Arg, i + 1),
                { Name: "nop" } => (accumulator, i + 1),
                { Name: "jmp" } => (accumulator, i + instruction.Arg),
                _ => throw new Exception("unknown instruction")
            };
        }

        return (accumulator, i);
    }
}