using System.Collections.Immutable;

using Registers;

using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static ImmutableList<Instruction> instructions = File.ReadLines("input.txt").Select(Instruction.Parse).ToImmutableList();
    internal static Result Part1() => Run(() => new Cpu(instructions).Run().MaxCurrentValue());
    internal static Result Part2() => Run(() => new Cpu(instructions).Run().MaxValueEver());
}
