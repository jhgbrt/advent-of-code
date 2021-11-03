using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace hexgrid
{
    class Program
    {
        static void Main(string[] args)
        {
            var steps = File.ReadLines("input.txt").SelectMany(l => l.Split(','));
            Run(() => HexGrid.Calculate(steps.ToArray()));
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
