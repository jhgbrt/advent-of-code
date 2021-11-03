using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Math;

class Program
{
    static void Main(string[] args)
    {
        var program = (
            from line in File.ReadLines("input.txt")
            let s = line.Split()
            select (instruction: s[0], arg1: s[1], arg2: s[2])
        ).ToList().AsReadOnly();

        Run(() => Evaluate("abcdefgh".ToDictionary(c => c, _ => 0), program));

        Run(() => Between(109900, 126900, 17).Count(NotPrime));

        bool NotPrime(int n) => !IsPrime(n);
        bool IsPrime(int n) => n != 1 && (n == 2 || n % 2 != 0 && Between(3, (int) Ceiling(Sqrt(n)), 2).All(i => n % i != 0));

        IEnumerable<int> Between(int lowerbound, int upperbound, int step)
        {
            for (var i = lowerbound; i <= upperbound; i += step) yield return i;
        }
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

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
