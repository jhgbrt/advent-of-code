using System;
using System.Diagnostics;
using System.IO;

namespace Garbage
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(() => new GarbageProcessor().ProcessFile("input.txt"));
        }

        static void Run<T>(Func<T> f)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} in {stopwatch.Elapsed}");
        }
    }
}
