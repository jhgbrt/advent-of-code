namespace AdventOfCode.Year2018.Day16;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));
    public static int Part1(string[] input)
    {
        var q = from sample in input.Parse()
                from opcode in OpCode.All
                let result = OpCode.apply(sample.before, sample.instruction, opcode)
                where result.SequenceEqual(sample.after)
                group sample by sample into g
                where g.Count() >= 3
                select g;

        return q.Count();
    }

    public static int Part2(string[] input)
    {
        var notmatching = (
            from sample in input.Parse()
            let code = sample.instruction.code
            group sample by code into sampleGroup
            from candidate in OpCode.All
            from sample in sampleGroup
            let result = OpCode.apply(sample.before, sample.instruction, candidate)
            where !result.SequenceEqual(sample.after)
            select (sampleGroup.Key, candidate)
            ).Distinct().ToLookup(x => x.Key, x => x.candidate);

        var opcodes = OpCode.All.ToList();
        var actualOpcodes = new Dictionary<int, Action<int[], int, int, int, int>>();
        do
        {
            var grp = notmatching.First(g => g.Count() == opcodes.Count - 1);
            var opcode = opcodes.Except(grp).Single();
            opcodes = opcodes.Where(o => o != opcode).ToList();
            notmatching = (from sample in notmatching
                           where sample.Key != grp.Key
                           from item in sample
                           where item != opcode
                           select (sample.Key, item))
                .ToLookup(x => x.Key, x => x.item);
            actualOpcodes[grp.Key] = opcode;
        } while (notmatching.Any());

        var missing = Enumerable.Range(0, 16).Where(i => !actualOpcodes.ContainsKey(i)).Single();

        actualOpcodes[missing] = opcodes.Single();

        var registers = new int[4];
        foreach (var instruction in input.GetInstructions())
        {
            var opcode = actualOpcodes[instruction.code];
            registers = OpCode.apply(registers, instruction, opcode);
        }


        return registers[0];
    }
}