namespace AdventOfCode.Year2017.Day08;

public class AoC201708
{
    static ImmutableList<Instruction> instructions = Read.InputLines(typeof(AoC201708)).Select(Instruction.Parse).ToImmutableList();
    public object Part1() => new Cpu(instructions).Run().MaxCurrentValue();
    public object Part2() => new Cpu(instructions).Run().MaxValueEver();
}