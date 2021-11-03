using System;
using System.Diagnostics;

namespace generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var seedA = 722;
            var seedB = 354;
            Run(() => Generator.GetNofMatches(seedA, seedB, 40_000_000));
            Run(() => Generator.GetNofMatches(seedA, seedB, 5_000_000, 4, 8));
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
