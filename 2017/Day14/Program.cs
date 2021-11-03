using System;
using System.Diagnostics;
using System.Numerics;

namespace defrag
{
    class Program
    {
        static void Main(string[] args)
        {
            string key = "hxtvlmkl";
            Run(() => Defrag.CountBitsInGrid(key));
            Run(() => Defrag.CountRegions(key));
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
