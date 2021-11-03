using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Jump
{
    public class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadLines("input.txt").Select(int.Parse).ToArray();
            Run(() => Jumps.CalculateJumps(input, v => 1));
            Run(() => Jumps.CalculateJumps(input, v => v >= 3 ? -1 : 1));
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} / {sw.Elapsed}");

        }
    }
}
