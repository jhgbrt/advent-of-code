using System;
using System.Diagnostics;

namespace AdventOfCode
{
    static class Program
    {
        public static void Main()
        {
            var input = 5177;
            Measure(() => AoC.Part1(input));
            Measure(() => AoC.Part2(input));
        }

        static void Measure<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"result = {result} - {sw.Elapsed}");
        }
        
    }
}
