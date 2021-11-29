using System.Collections.ObjectModel;

using static System.Math;

namespace AdventOfCode.Year2017.Day23;

partial class AoC
{
    static ReadOnlyCollection<(string instruction, string arg1, string arg2)> program = (
        from line in File.ReadLines("input.txt")
        let s = line.Split()
        select (instruction: s[0], arg1: s[1], arg2: s[2])
    ).ToList().AsReadOnly();

    internal static Result Part1() => Run(() => Evaluate("abcdefgh".ToDictionary(c => c, _ => 0), program));

    internal static Result Part2() => Run(() => Between(109900, 126900, 17).Count(NotPrime));

    static bool NotPrime(int n) => !IsPrime(n);
    static bool IsPrime(int n) => n != 1 && (n == 2 || n % 2 != 0 && Between(3, (int)Ceiling(Sqrt(n)), 2).All(i => n % i != 0));

    static IEnumerable<int> Between(int lowerbound, int upperbound, int step)
    {
        for (var i = lowerbound; i <= upperbound; i += step) yield return i;
    }
    private static int Evaluate(IDictionary<char, int> memory, IReadOnlyList<(string instruction, string arg1, string arg2)> program)
    {
        char Register(string s) => s[0];
        int Value(IDictionary<char, int> mem, string s) => char.IsLetter(s[0]) ? mem[s[0]] : int.Parse(s);
        int i = 0;
        int nofmultiplications = 0;
        while (i >= 0 && i < program.Count)
        {
            var instruction = program[i];
            switch (instruction.instruction)
            {
                case "set":
                    {
                        memory[Register(instruction.arg1)] = Value(memory, instruction.arg2);
                        i++;
                        break;
                    }
                case "sub":
                    {
                        memory[Register(instruction.arg1)] -= Value(memory, instruction.arg2);
                        i++;
                        break;
                    }
                case "mul":
                    {
                        nofmultiplications++;
                        memory[Register(instruction.arg1)] *= Value(memory, instruction.arg2);
                        i++;
                        break;
                    }
                case "jnz":
                    {
                        if (char.IsDigit(instruction.arg1[0]) || memory[Register(instruction.arg1)] != 0)
                            i += Value(memory, instruction.arg2);
                        else
                            i++;
                        break;
                    }
            }
        }
        return nofmultiplications;
    }
}