namespace AdventOfCode.Year2017.Day08;

public class AoCImpl : AoCBase
{
    static ImmutableList<Instruction> instructions = Read.InputLines(typeof(AoCImpl)).Select(Instruction.Parse).ToImmutableList();
    public override object Part1() => new Cpu(instructions).Run().MaxCurrentValue();
    public override object Part2() => new Cpu(instructions).Run().MaxValueEver();
}