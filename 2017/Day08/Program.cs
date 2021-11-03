using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Registers
{
    class Program
    {
        static void Main(string[] args)
        {
            var instructions = File.ReadLines("input.txt").Select(Instruction.Parse).ToImmutableList();

            Run(() => new Cpu(instructions).Run().MaxCurrentValue());
            Run(() => new Cpu(instructions).Run().MaxValueEver());
        }

        public static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
